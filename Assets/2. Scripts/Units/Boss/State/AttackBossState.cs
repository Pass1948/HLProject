using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackBossState : BaseBossState
{
    private int minRange => controller.minAttackRange;
    private int maxRange => controller.maxAttackRange;

    private List<GameObject> previewTiles = new List<GameObject>();
    
    public AttackBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Attack boss state");
        ShowAttackPreview(controller.GridPos, minRange, maxRange);
        controller.StartCoroutine(AttackRoutine());
    }

    public override void Exit()
    {

    }

    public override void Excute()
    {

    }
    
    
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f);

        Vector2Int playerPos2D = GameManager.Map.GetPlayer2Position();
        Vector3Int playerPos = new Vector3Int(playerPos2D.x, playerPos2D.y, 0);

        int dist = Mathf.Abs(controller.GridPos.x - playerPos.x) + Math.Abs(controller.GridPos.y - playerPos.y);

        if (dist >= minRange && dist <= maxRange)
        {
            GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, controller.model.attack);
            animHandler.OnAttack(GameManager.Unit.Player.transform);
        }

        ClearPreveiwTiles();

        stateMachine.ChangeState(stateMachine.EndState);
    }

    private void ShowAttackPreview(Vector3Int center, int minRange, int maxRange)
    {
        ClearPreveiwTiles();

        Tilemap overlayTileMap = GameManager.Map.moveInfoTilemap;
        overlayTileMap.ClearAllTiles();
        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                int dist = Mathf.Abs(x) + Mathf.Abs(y);
                if (dist >= minRange && dist <= maxRange)
                {
                    Vector3Int cell = new Vector3Int(center.x + x, center.y + y, 0);
                    GameObject tile = GameManager.Resource.Create<GameObject>(Path.Map + "RedTile");
                    //overlayTileMap.SetTile(cell, overlayTile);
                    Vector3 worldPos = overlayTileMap.GetCellCenterWorld(cell);
                    tile.transform.position = worldPos;
                    previewTiles.Add(tile);
                }
            }
        }
    }

    private void ClearPreveiwTiles()
    {
        foreach (var obj in previewTiles)
        {
            GameObject.Destroy(obj);
        }
        previewTiles.Clear();
    }
}
