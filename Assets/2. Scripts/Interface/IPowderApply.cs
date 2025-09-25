using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowderApply
{
    /// <summary>공격 시작 시 1회 호출</summary>
    void OnAttackStart(AttackCtx ctx);

    /// <summary>각 타깃을 타격하기 직전 호출. damage를 수정할 수 있습니다.</summary>
    void OnBeforeHit(AttackCtx ctx, BaseEnemy enemy, ref int damage);

    /// <summary>각 타깃을 타격한 직후 호출(보상 처리 등)</summary>
    void OnAfterHit(AttackCtx ctx, BaseEnemy enemy, int damage, bool killed);

    /// <summary>공격 종료 시 1회 호출</summary>
    void OnAttackEnd(AttackCtx ctx);
}

public class AttackCtx
{
    public PlayerModel player;             // 공격자
    public List<BaseEnemy> targets;        // 이번 공격 타깃들(샷건: 여러 명)
    public int baseDamage;                 // 기본 공격력(효과에서 증/감 가능)
    public int ammoFlag;                   // 기존에 쓰던 탄약 플래그(turnSetVlaue.fireAmmo 등)

    // 선택 항목(보상/로그 등 확장 포인트)
    public int bonusCoins;                 // 효과가 보상 추가 시 누적
    public System.Random rnd;              // 고급 난수 필요 시(원하면 Random.value 사용 가능)
}