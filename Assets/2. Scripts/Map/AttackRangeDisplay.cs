using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class AttackRangeDisplay : MonoBehaviour
{
    private Tilemap attackRangeTilemap;
    private TileBase rangeTile;
    private Camera mainCamera;
    public bool isInitialized = false;

    public Grid grid;

    public int range_plus = 0;
    
    private List<Vector3Int> currentAttackTiles = new List<Vector3Int>();

    private Suit currentSuit = Suit.Spade;
    private int currentRank = 0;
    
    private bool isKickAttack = false;

    public List<BaseEnemy> enemies = new List<BaseEnemy>();
    public List<BaseBoss> bosses = new List<BaseBoss>();


    void Update()
    {
        if (!isInitialized || (currentRank == 0 && !isKickAttack))
        {
            // ClearRange();
            GameManager.Map.ClearAttackTargets();
            return;
        }

        Vector3Int direction = GetDirectionFromMouse();
        // 포인트가 플레이어의 위치라면 업데이트 x
        if (direction == Vector3Int.zero)
        {
            return;
        }
        
        List<Vector3Int> newRange = new List<Vector3Int>();

        if (isKickAttack)
        {
            newRange = GetKickRange(direction);
        }
        else
        {
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
        }
        ShowRange(newRange);
        // 범위를 MapManager로 보내 타겟 목록을 갱신
        GameManager.Map.UpdateAttackTargets(newRange, enemies);
    }


    public void SetAttackRange(Suit suit, int rank) // 기본공격 <- 이걸 버튼에 연결
    {
        if (currentSuit == suit && currentRank == rank)
        {
            ClearAttackType();
        }
        else
        {
            currentSuit = suit;
            currentRank = rank;
            isKickAttack = false;
        }
    }
    
    public void SetAttackRangeForKick() // 킥 공격 <- 이걸 버튼에 연결
    {
        isKickAttack = true; 
        currentRank = 0;
    }

    public void ClearAttackType() // 범위 지우기
    {
        currentSuit = Suit.Spade;
        currentRank = 0;
        isKickAttack = false;
        ClearRange();
        GameManager.Map.ClearAttackTargets();
    }

    public void Initialize(Tilemap attackTilemap, TileBase rangeTileBase, Grid gridObj)
    {
        if (isInitialized) return;
        this.attackRangeTilemap = attackTilemap;
        this.rangeTile = rangeTileBase;
        this.grid = gridObj;

        attackRangeTilemap.transform.localPosition = Vector3.zero;

        isInitialized = true;
    }

    public void ShowRange(List<Vector3Int> rangeCells)
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

    private Vector3Int GetPlayerCellPosition()
    {
        Vector2Int playerPos2D = GameManager.Map.GetPlayer2Position();
        return new Vector3Int(playerPos2D.x, playerPos2D.y, 0);
    }

    private Vector3Int GetDirectionFromMouse()
    {
        if (GameManager.Mouse == null || grid == null || GameManager.Mouse.tilemap == null)
        {
            return Vector3Int.zero;
        }

        Vector3Int playerCellPos = GetPlayerCellPosition();
        Vector3Int mouseCellPos = GameManager.Mouse.PointerCell;
        
        Vector3Int relativePos = mouseCellPos - playerCellPos;
        
        if (relativePos == Vector3Int.zero)
        {
            return Vector3Int.zero;
        }
        
        if (relativePos.x == 0 && relativePos.y > 0) return Vector3Int.up;
        if (relativePos.x > 0 && relativePos.y == 0) return Vector3Int.right;
        if (relativePos.x == 0 && relativePos.y < 0) return Vector3Int.down;
        if (relativePos.x < 0 && relativePos.y == 0) return Vector3Int.left;
        
        if (relativePos.x > 0 && relativePos.y > 0) // 오른쪽 상단
        {
            return Vector3Int.up;
        }
        else if (relativePos.x > 0 && relativePos.y < 0) // 오른쪽 하단
        {
            return Vector3Int.right;
        }
        else if (relativePos.x < 0 && relativePos.y < 0) // 왼쪽 하단
        {
            return Vector3Int.down;
        }
        else // 왼쪽 상단
        {
            return Vector3Int.left;
        }
    }

    private List<Vector3Int> GetDiamondRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();
        int effectiveRange = 2 + GameManager.Unit.Player.playerModel.attackRange;
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
        
        if (direction.x > 0)
        {
            diagonalDirection = new Vector3Int(1, -1, 0);
        }
        else if (direction.x < 0)
        {
            diagonalDirection = new Vector3Int(-1, 1, 0);
        }
        else if (direction.y > 0)
        {
            diagonalDirection = new Vector3Int(1, 1, 0);
        }
        else
        {
            diagonalDirection = new Vector3Int(-1, -1, 0);
        }
        
        int effectiveRange = 2 + GameManager.Unit.Player.playerModel.attackRange;
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
        int effectiveRange = 1 + GameManager.Unit.Player.playerModel.attackRange;

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

        for (int i = 1; i <= 1 + GameManager.Unit.Player.playerModel.attackRange; i++)
        {
            range.Add(playerPos + diagLeft * i);
            range.Add(playerPos + diagRight * i);
        }

        return range;
    }
    
    private List<Vector3Int> GetKickRange(Vector3Int direction)
    {
        Vector3Int playerPos = GetPlayerCellPosition();
        var range = new List<Vector3Int>();
        
        range.Add(playerPos + direction);
        
        return range;
    }
}