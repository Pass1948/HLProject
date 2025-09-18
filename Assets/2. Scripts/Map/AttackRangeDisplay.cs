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
    
    public GameObject grid;
    
    public int range_plus = 0;

    private Vector3Int playerCellPosition;
    private List<Vector3Int> currentAttackTiles = new List<Vector3Int>();
    
    public Vector3Int clickAttackTile { get; private set; }

    // 모양(Suit)과 숫자(Rank)로 구분하여 저장
    private Suit currentSuit = Suit.Spade;
    private int currentRank = 0;
    
    void Update()
    {
        if (!isInitialized || currentRank == 0)
        {
            ClearRange();
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
        
    }
    

    // 공격 타입을 설정
    public void SetAttackRange(Suit suit, int rank)
    {
        // 카드가 같다면, 공격 범위를 해제
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
    }
    
    public void Initialize(Tilemap attackTilemap, TileBase rangeTileBase, Camera cam, GameObject gridObj)
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
    
    //
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
    
    public void SetClickedTile(Vector3Int cell)
    {
        if (currentAttackTiles.Contains(cell))
        {
            clickAttackTile = cell;
        }
    }
    
    // 플레이어 위치 가져옴
    private Vector3Int GetPlayerCellPosition()
    {
        Vector2Int playerPos2D = GameManager.Map.GetPlayerPosition();
        return new Vector3Int(playerPos2D.x, playerPos2D.y, 0);
    }
    
    // 공격 방향을 계산
    private Vector3Int GetDirectionFromMouse()
    {
        Vector3Int playerCellPos = GetPlayerCellPosition();
        Vector2 playerWorldPos = grid.GetComponent<Grid>().CellToWorld(playerCellPos);
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(playerWorldPos);
        
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 screenDistanceVector = mouseScreenPos - playerScreenPos;

        // === 디버그 로그 ===
        Debug.Log($"화면 기준 거리 차이: {screenDistanceVector}");
    
        // 4. 가로/세로 거리 중 더 큰 값을 기준으로 방향을 결정합니다.
        Vector3Int finalDirection;

        if (Mathf.Abs(screenDistanceVector.x) > Mathf.Abs(screenDistanceVector.y))
        {
            finalDirection = (screenDistanceVector.x > 0) ? Vector3Int.right : Vector3Int.left;
        }
        else
        {
            finalDirection = (screenDistanceVector.y > 0) ? Vector3Int.up : Vector3Int.down;
        }
    
        return finalDirection;
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
        
        Vector3Int diagonalDirection;
        
        if (direction.x != 0)
        {
            diagonalDirection = new Vector3Int(direction.x, direction.x, 0);
        }
        else
        {
            diagonalDirection = new Vector3Int(direction.y, direction.y, 0);
        }

        int effectiveRange = 2 + range_plus;
        for (int i = 1; i <= effectiveRange; i++)
        {
            range.Add(playerPos + diagonalDirection * i);
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