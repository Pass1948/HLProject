using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class StrategyManager : MonoBehaviour
{
    // =====================================================================
    // 캐싱 변수들
    // =====================================================================
    private ItemControlManger itemControl = GameManager.ItemControl;
    private PlayerModel player = GameManager.Unit.Player.playerModel;
    private List<ItemModel> relicStatItems = GameManager.ItemControl.relicStatItems;
    private List<ItemModel> relicRogicItems = GameManager.ItemControl.relicRogicItems;
    private List<ItemModel> powderStatItems = GameManager.ItemControl.powderStatItems;
    private List<ItemModel> powderRogicItems = GameManager.ItemControl.powderRogicItems;
    
    private Ammo fireAmmo;
    
    
    private int _attackCount;
    public void ResetCounters() => _attackCount = 0;

    private bool IsNthAttack(int n) => (_attackCount % n) == (n - 1);

    // ===== 기존 공개 필드(전역 선택 화약) =====
    public int currentPowderItemId;
    
    // =====================================================================
    // 유물 아이템 스탯 효과 로직들
    // =====================================================================
    
    public void AttackEnemy()
    {
        // 범위내에 있는 적들 전원 공격
        var targets = GameManager.Map.CurrentEnemyTargets;
        if (targets != null && targets.Count > 0)
        {
            foreach (var enemy in targets)
            {
                if (enemy == null || enemy.controller == null || enemy.controller.isDie) continue;
                GameManager.Unit.ChangeHealth(
                    enemy.enemyModel,
                    GameManager.Unit.Player.playerModel.attack,
                    fireAmmo
                );
                enemy.controller.OnHitState();
            }
        }
    }
    
     public void ExecuteNow()
    {
        // 1) 플레이어/데미지
        var player = GameManager.Unit?.Player?.playerModel;
        if (player == null) return;
        int baseDamage = player.attack;

        // 2) 탄 속성(프로젝트 값에 맞게 가져오세요. 예: TurnSettingValue.fireAmmo)
        //    turnSetVlaue / turnSettingValue 중 실제 쓰는 쪽으로 교체
        var ammoFlag = GameManager.TurnBased?.turnSettingValue != null
            ? GameManager.TurnBased.turnSettingValue.fireAmmo
            : default;

        // 3) 현재 공격 범위의 타깃들(이미 MapManager가 모아둔 리스트 사용)
        var targets = GameManager.Map?.CurrentEnemyTargets;
        if (targets == null || targets.Count == 0) return;

        // 4) 화약 아이템(필요시 최소 스텁 생성)
        var powderItem = MakePowderStub(currentPowderItemId);

        // 5) 실제 실행
        ExecutePowderAttack(powderItem, player, targets, baseDamage, ammoFlag);
    }

    // ====== [추가] 외부 1회 호출용 파사드(타깃만 전달) ======
    // - 타깃만 바꾸고, 나머진 자동으로 가져와 실행하고 싶을 때 사용
    public void ExecuteNow(List<BaseEnemy> overrideTargets)
    {
        var player = GameManager.Unit?.Player?.playerModel;
        if (player == null) return;
        int baseDamage = player.attack;

        var ammoFlag = GameManager.TurnBased?.turnSettingValue != null
            ? GameManager.TurnBased.turnSettingValue.fireAmmo
            : default;

        if (overrideTargets == null || overrideTargets.Count == 0) return;

        var powderItem = MakePowderStub(currentPowderItemId);

        ExecutePowderAttack(powderItem, player, overrideTargets, baseDamage, ammoFlag);
    }

    // ===== 기존 시그니처 유지 =====
    public void ExecutePowderAttack(
        ItemModel powderItem, PlayerModel player,
        List<BaseEnemy> targets, int baseDamage, Ammo ammoFlag)
    {
        if (player == null || targets == null || targets.Count == 0) return;

        // 공격 “발생” 순간 카운터 증가 (7번째 공격 규칙용)
        _attackCount++;
        bool isCritByRule = IsNthAttack(7);

        // (필요시) 화약 효과에 따른 피해 조정 로직이 있다면 여기서 적용
        // ex) dmg = ApplyPowderModifiers(powderItem, baseDamage, ...)

        for (int i = 0; i < targets.Count; i++)
        {
            var enemy = targets[i];
            if (enemy == null || enemy.controller == null || enemy.controller.isDie) continue;

            int dmg = baseDamage;

            // 7번째 공격 치명타(예: x2)
            if (isCritByRule)
                dmg = Mathf.RoundToInt(dmg * 2f);

            if (dmg > 0)
            {
                GameManager.Unit.ChangeHealth(enemy.enemyModel, dmg, ammoFlag);
                enemy.controller.OnHitState();
            }
        }
    }
    private ItemModel MakePowderStub(int id)
    {
        var m = new ItemModel();
        m.id = id;
        return m;
    }

}