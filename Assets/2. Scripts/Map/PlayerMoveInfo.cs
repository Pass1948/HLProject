using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveInfo : MonoBehaviour
{
    // BFS를 이용한 이동 가능 범위 표시
    public void ShowMoveInfoRange(Vector3Int playerPos, int moveRange, TileBase overlayTile, Tilemap overlayTilemap)
    {
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;

        // BFS 준비
        Queue<(Vector3Int pos, int dist)> queue = new Queue<(Vector3Int, int)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        // 시작점 설정.(시작위치)
        queue.Enqueue((playerPos, 0));
        visited.Add(playerPos);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            // 범위 내라면 타일 표시
            if (dist <= moveRange)
            {
                overlayTilemap.SetTile(current, overlayTile);

                // 다음 칸을 moveRange 만큼 탐색.
                if (dist < moveRange)
                {
                    Vector3Int[] dirs = {

                        new Vector3Int(1, 0, 0),
                        new Vector3Int(-1, 0, 0),
                        new Vector3Int(0, 1, 0),
                        new Vector3Int(0, -1, 0),
                     };

                    foreach (var dir in dirs)
                    {
                        Vector3Int next = current + dir;

                        // 맵 범위 밖
                        if (next.x < 0 || next.x >= mapWidth ||
                            next.y < 0 || next.y >= mapHeight)
                            continue;
                        if(visited.Contains(next)) continue;


                        // 맵 범위 안 & 아직 방문 안 함 & Terrain일 때
                       if(GameManager.Map.IsMovable(next))
                        {
                            visited.Add(next);
                            queue.Enqueue((next, dist + 1));
                        }
                    }
                }
            }
        }
    }
    public void RemoveMoveInfoRange(Tilemap overlayTilemap)
    {
        overlayTilemap.ClearAllTiles();
    }
}