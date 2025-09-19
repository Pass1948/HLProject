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

    public void ClearRange()
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
        if (mainCamera == null || grid == null)
        {
            return Vector3Int.zero;
        }
        Vector3Int playerCellPos = GetPlayerCellPosition();
        Vector2 playerWorldPos = grid.GetComponent<Grid>().CellToWorld(playerCellPos);
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(playerWorldPos);

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 screenDistanceVector = mouseScreenPos - playerScreenPos;

        // 거리가 너무 짧으면 방향을 정하지 않음
        if (screenDistanceVector.magnitude < 1) // 임계값은 적당히 수정
        {
            return Vector3Int.zero;
        }

        // 마우스와 플레이어 간의 각도를 계산
        float angle = Mathf.Atan2(screenDistanceVector.y, screenDistanceVector.x) * Mathf.Rad2Deg;

        // 각도가 0~360도 범위에 있도록 조정
        angle -= 20f;
        if (angle < 0)
        {
            angle += 360;
        }

        // 각도를 기준으로 4방향(상,하,좌,우) 중 하나 
        if (angle >= 45f && angle < 135f) // 45 ~ 135 (위쪽)
        {
            return Vector3Int.up;
        }
        else if (angle >= 135f && angle < 225f) // 135 ~ 225 (왼쪽)
        {
            return Vector3Int.left;
        }
        else if (angle >= 225f && angle < 315f) // 225 ~ 315 (아래쪽)
        {
            return Vector3Int.down;
        }
        else // 315 ~ 360 or 0 ~ 45 (오른쪽)
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