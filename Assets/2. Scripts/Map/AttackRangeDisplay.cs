using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class AttackRangeDisplay : MonoBehaviour
{
    private Tilemap attackRangeTilemap;
    private TileBase rangeTile;
    private Camera mainCamera;
    private bool isInitialized = false;

    public Grid grid;

    public int range_plus = 0;

    private List<Vector3Int> currentAttackTiles = new List<Vector3Int>();

    private Suit currentSuit = Suit.Spade;
    private int currentRank = 0;
    
    public List<BaseEnemy> enemies = new List<BaseEnemy>();


    void Update()
    {
        if (!isInitialized || currentRank == 0)
        {
            ClearRange();
            GameManager.Map.ClearAttackTargets();
            return;
        }

        Vector3Int direction = GetDirectionFromMouse();
        List<Vector3Int> newRange = new List<Vector3Int>();

        switch (currentSuit)
        {
            case Suit.Diamond:
                newRange = GetDiamondRange(direction);
                break;
            case Suit.Heart:
                newRange = GetHeartRange(direction);
                break;
            case Suit.Spade:
                newRange = GetSpadeRange(direction);
                break;
            case Suit.Club:
                newRange = GetCloverRange(direction);
                break;
        }

        ShowRange(newRange);
        
        // 범위를 MapManager로 보내 타겟 목록을 갱신
        GameManager.Map.UpdateAttackTargets(newRange, enemies);
    }

    public void SetAttackRange(Suit suit, int rank)
    {
        if (currentSuit == suit && currentRank == rank)
        {
            ClearAttackType();
        }
        else
        {
            currentSuit = suit;
            currentRank = rank;
        }
    }

    public void ClearAttackType()
    {
        currentSuit = Suit.Spade;
        currentRank = 0;
        ClearRange();
        GameManager.Map.ClearAttackTargets();
    }

    public void Initialize(Tilemap attackTilemap, TileBase rangeTileBase, Camera cam, Grid gridObj)
    {
        if (isInitialized) return;

        grid = gridObj;
        mainCamera = cam;
        rangeTile = rangeTileBase;

        attackRangeTilemap = attackTilemap;
        attackRangeTilemap.transform.SetParent(grid.transform);
        attackRangeTilemap.transform.localPosition = Vector3.zero;

        isInitialized = true;
    }

    private void ShowRange(List<Vector3Int> rangeCells)
    {
        if (attackRangeTilemap != null)
        {
            ClearRange();
            currentAttackTiles = rangeCells;

            foreach (var cell in currentAttackTiles)
            {
                attackRangeTilemap.SetTile(cell, rangeTile);
            }
        }
    }

    private void ClearRange()
    {
        if (attackRangeTilemap != null)
        {
            attackRangeTilemap.ClearAllTiles();
        }
    }

    private Vector3Int GetPlayerCellPosition()
    {
        Vector2Int playerPos2D = GameManager.Map.GetPlayerPosition();
        return new Vector3Int(playerPos2D.x, playerPos2D.y, 0);
    }

    private Vector3Int GetDirectionFromMouse()
    {
        if (mainCamera == null || grid == null)
        {
            return Vector3Int.zero;
        }

        Vector3Int playerCellPos = GetPlayerCellPosition();
        Vector2 playerWorldPos = grid.GetComponent<Grid>().CellToWorld(playerCellPos);
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(playerWorldPos);

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 screenDistanceVector = mouseScreenPos - playerScreenPos;

        if (screenDistanceVector.magnitude < 1)
        {
            return Vector3Int.zero;
        }

        float angle = Mathf.Atan2(screenDistanceVector.y, screenDistanceVector.x) * Mathf.Rad2Deg;
        angle -= 20f;
        if (angle < 0)
        {
            angle += 360;
        }

        if (angle >= 45f && angle < 135f)
        {
            return Vector3Int.up;
        }
        else if (angle >= 135f && angle < 225f)
        {
            return Vector3Int.left;
        }
        else if (angle >= 225f && angle < 315f)
        {
            return Vector3Int.down;
        }
        else
        {
            return Vector3Int.right;
        }
    }

    private List<Vector3Int> GetDiamondRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();
        int effectiveRange = 2 + range_plus;
        for (int i = 1; i <= effectiveRange; i++)
        {
            range.Add(playerPos + direction * i);
        }
        return range;
    }

    private List<Vector3Int> GetHeartRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();

        Vector3Int leftDirection = new Vector3Int(-direction.y, direction.x, 0);
        Vector3Int rightDirection = new Vector3Int(direction.y, -direction.x, 0);

        int effectiveRange = 2 + range_plus;
        for (int i = 1; i <= effectiveRange; i++)
        {
            range.Add(playerPos + leftDirection * i);
            range.Add(playerPos + rightDirection * i);
        }
        return range;
    }

    private List<Vector3Int> GetSpadeRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();
        int effectiveRange = 1 + range_plus;

        for (int i = 1; i <= effectiveRange; i++)
        {
            range.Add(playerPos + direction * i);
        }

        Vector3Int leftDirection = new Vector3Int(-direction.y, direction.x, 0);
        Vector3Int rightDirection = new Vector3Int(direction.y, -direction.x, 0);

        range.Add(playerPos + leftDirection);
        range.Add(playerPos + rightDirection);

        return range;
    }

    private List<Vector3Int> GetCloverRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();

        range.Add(playerPos + direction);

        Vector3Int diagLeft = new Vector3Int(direction.x - direction.y, direction.y + direction.x, 0);
        Vector3Int diagRight = new Vector3Int(direction.x + direction.y, direction.y - direction.x, 0);

        for (int i = 1; i <= 1 + range_plus; i++)
        {
            range.Add(playerPos + diagLeft * i);
            range.Add(playerPos + diagRight * i);
        }

        return range;
    }
}