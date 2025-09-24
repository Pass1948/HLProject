using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StatusEffectSystem :  MonoBehaviour
{

   // ====== 외부 바인딩(에디터에서 Drag&Drop 추천) ======
    [Header("Bindings")]
    [SerializeField] private UnitModel model;     // PlayerModel/EnemyModel (공통 베이스)
    [SerializeField] private BasePlayer player;   // 플레이어인 경우 할당
    [SerializeField] private BaseEnemy enemy;     // 적인 경우 할당
    [SerializeField] private MapManager map;      // GameManager.Map
    [SerializeField] private UnitManager units;   // GameManager.Unit

    // 에디터에서 누락 시 보조 바인딩(Find 아님, 저비용)
    private void Awake()
    {
        if (model != null)
        {
            player = player ? player : GetComponent<BasePlayer>();
            enemy = enemy ? enemy : GetComponent<BaseEnemy>();
            if (player) model = player.playerModel;
            else if (enemy) model = enemy.enemyModel;
        }

        if (!map)   map   = GameManager.Map;
        if (!units) units = GameManager.Unit;

        // 컨텍스트 1회 생성 후 재사용(매 프레임 new 방지)
        _ctx = new StatusContext();
    }

    // ====== 상태 식별/순서 ======
    public enum StatusId
    {
        Stun,      // 기절(불가중첩, 턴 종료 제거)
        Bleed,     // 출혈(가능, 턴 종료 피해 후 제거)
        Burn,      // 화상(가능, 매턴 시작 피해, 턴 종료 스택-1)
        Regen,     // 재생(가능, 턴 시작 회복 후 바로 제거)
        Slow,      // 감속(가능, 스택당 이동-1)
        Haste,     // 가속(가능, 스택당 이동+1)
        Blind,     // 실명(불가중첩, 공격-1, 경로 미표시)
        Berserk,   // 광폭(가능, 스택당 공격+1)
        Freeze,    // 빙결(불가중첩, 이동-10)
        Confuse    // 혼란(불가중첩, 피격 시 공격 방향으로 1칸 밀림)
    }

    // 적용 순서 고정(요구 사양 그대로)
    private static readonly StatusId[] APPLY_ORDER = new[]
    {
        StatusId.Stun, StatusId.Bleed, StatusId.Burn, StatusId.Regen,
        StatusId.Slow, StatusId.Haste, StatusId.Blind, StatusId.Berserk,
        StatusId.Freeze, StatusId.Confuse
    };

    // ====== 내부 저장 구조(가벼운 클래스 + 재사용 버퍼) ======
    [Serializable]
    private sealed class StatusInstance
    {
        public StatusId id;
        public int stacks;
        public bool stackable;
        public bool removeAtTurnStart;
        public bool removeAtTurnEnd;

        public StatusInstance(StatusId id, int stacks, bool stackable, bool rmStart, bool rmEnd)
        { this.id = id; this.stacks = stacks; this.stackable = stackable; this.removeAtTurnStart = rmStart; this.removeAtTurnEnd = rmEnd; }
    }

    private readonly Dictionary<StatusId, StatusInstance> _active = new(16);
    private readonly List<StatusId> _removeBuf = new(8);

    // ====== 컨텍스트(Find 없이 필요한 레퍼런스 모음) ======
    private sealed class StatusContext
    {
        public UnitModel model;
        public BasePlayer player;
        public BaseEnemy enemy;
        public MapManager map;
        public UnitManager units;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDead() => (model == null) || model.currentHealth <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TrueDamage(int amt)
        {
            if (model == null || amt <= 0) return;
            model.currentHealth = Mathf.Max(0, model.currentHealth - amt);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Heal(int amt)
        {
            if (model == null || amt <= 0) return;
            model.currentHealth = Mathf.Min(model.maxHealth, model.currentHealth + amt);
        }
    }
    private StatusContext _ctx;

    // ====== 전략(효과 함수 테이블) ======
    // 델리게이트 시그니처
    private delegate void TurnFn(StatusContext ctx, StatusInstance inst);
    private delegate int  IntFn(StatusInstance inst);
    private delegate bool BoolFn(StatusInstance inst);
    private delegate void AttackedFn(StatusContext ctx, StatusInstance inst, Vector3Int dir);

    private sealed class Ops
    {
        public TurnFn OnStart;
        public TurnFn OnEnd;
        public IntFn  MoveDelta;
        public IntFn  AtkDelta;
        public BoolFn HidePath;
        public AttackedFn OnAttacked;
        public Ops(TurnFn os, TurnFn oe, IntFn md, IntFn ad, BoolFn hp, AttackedFn oa)
        { OnStart=os; OnEnd=oe; MoveDelta=md; AtkDelta=ad; HidePath=hp; OnAttacked=oa; }
    }

    // 효과 테이블(정적, 한 번만 생성 → GC 없음)
    private static readonly Dictionary<StatusId, Ops> OPS = new()
    {
        { StatusId.Stun, new Ops(Stun_Start, NoopEnd, Zero, Zero, False, NoopHit) },
        { StatusId.Bleed, new Ops(NoopStart, Bleed_End, Zero, Zero, False, NoopHit) },
        { StatusId.Burn, new Ops(Burn_Start, Burn_End, Zero, Zero, False, NoopHit) },
        { StatusId.Regen, new Ops(Regen_Start, NoopEnd, Zero, Zero, False, NoopHit) },
        { StatusId.Slow, new Ops(NoopStart, NoopEnd, Slow_Move, Zero, False, NoopHit) },
        { StatusId.Haste, new Ops(NoopStart, NoopEnd, Haste_Move, Zero, False, NoopHit) },
        { StatusId.Blind, new Ops(NoopStart, NoopEnd, Zero, Blind_Atk, True, NoopHit) },
        { StatusId.Berserk, new Ops(NoopStart, NoopEnd, Zero, Berserk_Atk, False, NoopHit) },
        { StatusId.Freeze, new Ops(NoopStart, NoopEnd, Freeze_Move, Zero, False, NoopHit) },
        { StatusId.Confuse, new Ops(NoopStart, NoopEnd, Zero, Zero, False, Confuse_OnHit) },
    };

    // ====== 효과 구현(정적 메서드: 캡처/할당 없음) ======
    private static void NoopStart(StatusContext c, StatusInstance i) { }
    private static void NoopEnd  (StatusContext c, StatusInstance i) { }
    private static int  Zero     (StatusInstance i) => 0;
    private static bool True    (StatusInstance i) => true;
    private static bool False    (StatusInstance i) => false;
    private static void NoopHit  (StatusContext c, StatusInstance i, Vector3Int d) { }

    // 기절: FSM에서 "행동 불가"를 체크하도록 Has(Stun) 질의 사용 추천
    private static void Stun_Start(StatusContext c, StatusInstance i)
    {
        // 필요 시 컨트롤러에 통지(예: c.player?.controller?.SetStunned(true))
        // 여기서는 플래그 없이, 외부에서 status.Has(Stun)으로 행동 제한하도록 권장.
    }

    // 출혈: 턴 종료에 스택만큼 피해
    private static void Bleed_End(StatusContext c, StatusInstance i)
    {
        if (i.stacks > 0) c.TrueDamage(i.stacks);
    }

    // 화상: 턴 시작 피해, 종료에 스택-1
    private static void Burn_Start(StatusContext c, StatusInstance i)
    {
        if (i.stacks > 0) c.TrueDamage(i.stacks);
    }
    private static void Burn_End(StatusContext c, StatusInstance i)
    {
        if (i.stacks > 0) i.stacks -= 1;
    }

    // 재생: 턴 시작 회복 후(정책상) 시작 즉시 제거
    private static void Regen_Start(StatusContext c, StatusInstance i)
    {
        if (i.stacks > 0) c.Heal(i.stacks);
    }

    // 감속/가속/실명/광폭/빙결 수치 보정
    private static int Slow_Move   (StatusInstance i) => -Mathf.Max(0, i.stacks);
    private static int Haste_Move  (StatusInstance i) =>  Mathf.Max(0, i.stacks);
    private static int Blind_Atk   (StatusInstance i) => -1;         // 불가중첩: -1 고정
    private static int Berserk_Atk (StatusInstance i) =>  Mathf.Max(0, i.stacks);
    private static int Freeze_Move (StatusInstance i) => -10;

    // 혼란: 피격 방향으로 1칸 밀림
    private static void Confuse_OnHit(StatusContext c, StatusInstance i, Vector3Int dir)
    {
        if (c.map == null || c.IsDead()) return;

        Transform t; Vector3Int cur;
        if (c.enemy != null) { t = c.enemy.transform;  cur = c.map.tilemap.WorldToCell(t.position); }
        else                  { t = c.player.transform; cur = c.map.tilemap.WorldToCell(t.position); }

        var next = new Vector3Int(cur.x + dir.x, cur.y + dir.y, 0);
        if (c.map.IsMovable(next))
            t.position = c.map.tilemap.GetCellCenterWorld(next);
    }

    // ====== 정책(스택/제거 타이밍) ======
    private static (bool stackable, bool rmStart, bool rmEnd) Policy(StatusId id)
    {
        return id switch
        {
            StatusId.Stun    => (false, false, true), // 종료 제거
            StatusId.Bleed   => (true,  false, true), // 종료 제거
            StatusId.Burn    => (true,  false, false),// 종료에서 스택만 감소(0되면 아래에서 제거)
            StatusId.Regen   => (true,  true,  false),// 시작 제거
            StatusId.Slow    => (true,  false, true),
            StatusId.Haste   => (true,  false, true),
            StatusId.Blind   => (false, false, true),
            StatusId.Berserk => (true,  false, true),
            StatusId.Freeze  => (false, false, true),
            StatusId.Confuse => (false, false, true),
            _ => (false, false, false)
        };
    }

    // ====== 외부 API ======
    public event Action OnStatusChanged;

    /// 상태 부여(스택 추가)
    public void Add(StatusId id, int stacks = 1)
    {
        if (stacks <= 0) return;
        var p = Policy(id);
        if (_active.TryGetValue(id, out var inst))
        {
            inst.stacks = p.stackable ? (inst.stacks + stacks) : 1;
        }
        else
        {
            _active[id] = new StatusInstance(id, stacks, p.stackable, p.rmStart, p.rmEnd);
        }
        OnStatusChanged?.Invoke();
    }

    /// 상태 제거(스택 차감; 기본은 전량 제거)
    public void Remove(StatusId id, int stacks = int.MaxValue)
    {
        if (!_active.TryGetValue(id, out var inst)) return;
        inst.stacks -= Mathf.Max(1, stacks);
        if (inst.stacks <= 0) _active.Remove(id);
        OnStatusChanged?.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has(StatusId id) => _active.ContainsKey(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetStacks(StatusId id) => _active.TryGetValue(id, out var i) ? i.stacks : 0;

    /// 이동/공격 보정 합산(경로/표시 전 산출)
    public int GetMoveDeltaAll()
    {
        int acc = 0;
        for (int i = 0; i < APPLY_ORDER.Length; i++)
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
                acc += OPS[APPLY_ORDER[i]].MoveDelta(inst);
        return acc;
    }
    public int GetAttackDeltaAll()
    {
        int acc = 0;
        for (int i = 0; i < APPLY_ORDER.Length; i++)
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
                acc += OPS[APPLY_ORDER[i]].AtkDelta(inst);
        return acc;
    }
    public bool ShouldHidePathPreview()
    {
        for (int i = 0; i < APPLY_ORDER.Length; i++)
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
                if (OPS[APPLY_ORDER[i]].HidePath(inst)) return true;
        return false;
    }

    /// 피격 리액션(혼란 등)
    public void OnAttacked(Vector3Int attackDirCellDelta)
    {
        PrepareCtx();
        for (int i = 0; i < APPLY_ORDER.Length; i++)
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
                OPS[APPLY_ORDER[i]].OnAttacked(_ctx, inst, attackDirCellDelta);
    }

    /// 턴 시작: "적용 → 시작 제거" (동시 발생 시 적용 우선)
    public void TickTurnStart()
    {
        if (model == null) return;
        PrepareCtx();

        // 적용
        for (int i = 0; i < APPLY_ORDER.Length; i++)
        {
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
            {
                OPS[APPLY_ORDER[i]].OnStart(_ctx, inst);
                if (_ctx.IsDead()) break; // 적용 중 사망 시 이후 스킵
            }
        }

        // 시작 제거
        _removeBuf.Clear();
        foreach (var kv in _active)
            if (kv.Value.removeAtTurnStart) _removeBuf.Add(kv.Key);
        for (int i = 0; i < _removeBuf.Count; i++) _active.Remove(_removeBuf[i]);

        if (_removeBuf.Count > 0) OnStatusChanged?.Invoke();
    }

    /// 턴 종료: "종료 처리 → 종료 제거(스택 0 포함)"
    public void TickTurnEnd()
    {
        if (model == null) return;
        PrepareCtx();

        // 종료 처리
        for (int i = 0; i < APPLY_ORDER.Length; i++)
        {
            if (_active.TryGetValue(APPLY_ORDER[i], out var inst))
            {
                OPS[APPLY_ORDER[i]].OnEnd(_ctx, inst);
                if (_ctx.IsDead()) break;
            }
        }

        // 종료 제거(스택0/정책)
        _removeBuf.Clear();
        foreach (var kv in _active)
        {
            var inst = kv.Value;
            if (inst.stacks <= 0 || inst.removeAtTurnEnd) _removeBuf.Add(kv.Key);
        }
        for (int i = 0; i < _removeBuf.Count; i++) _active.Remove(_removeBuf[i]);

        if (_removeBuf.Count > 0) OnStatusChanged?.Invoke();
    }

    // 컨텍스트 갱신(재사용 객체, new 없음)
    private void PrepareCtx()
    {
        _ctx.model  = model;
        _ctx.player = player;
        _ctx.enemy  = enemy;
        _ctx.map    = map;
        _ctx.units  = units;
    }
 
}
