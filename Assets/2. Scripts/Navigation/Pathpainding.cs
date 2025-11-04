using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Pathfinding
{
    private HashSet<Vector3Int> blocked;
    public Pathfinding(Tilemap tilemap)
    {
        tilemap = GameManager.Map.tilemap;
        blocked = new HashSet<Vector3Int>();

        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;
        int[,] mapData = GameManager.Map.mapData;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int id = mapData[x, y];

                // 벽, 적, 장애물 타일이면 막힌 좌표로 추가
                if (id == TileID.Wall || id == TileID.Enemy || id == TileID.Obstacle)
                {
                    blocked.Add(new Vector3Int(x, y, 0));
                }
            }
        }
    }

    public void ResetMapData()
    {
        blocked.Clear();
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;
        int[,] mapData = GameManager.Map.mapData;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int id = mapData[x, y];

                // 벽, 적, 장애물 타일이면 막힌 좌표로 추가
                if (id == TileID.Wall || id == TileID.Enemy || id == TileID.Obstacle || id == TileID.Vehicle)
                {
                    blocked.Add(new Vector3Int(x, y, 0));
                }
                
            }
        }
    }

    public void ResetMapDataPlayer()
    {
        blocked.Clear();
        int mapWidth = GameManager.Map.mapWidth;
        int mapHeight = GameManager.Map.mapHeight;
        int[,] mapData = GameManager.Map.mapData;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int id = mapData[x, y];

                // 벽, 적, 장애물 타일이면 막힌 좌표로 추가
                if (id == TileID.Wall || id == TileID.Enemy || id == TileID.Obstacle)
                {
                    blocked.Add(new Vector3Int(x, y, 0));
                }

            }
        }
    }


    // A * 알고리즘으로 start -> goal 까지의 경로를 구함

    // start
    // goal
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        int[,] mapdata = GameManager.Map.mapData;
        int startID = mapdata[start.x, start.y];

     
        //탐색 후보군 (openSet) 과 이미 방문한 집합 (closedSet)
        List<Node> openSet = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // 시작 노드 초기화(등록)
        Node startNode = new Node(start);
        openSet.Add(startNode);

        //전체 노드를 저장(같은 좌표의 노드를 재사용하기 위함)
        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

        allNodes[start] = startNode;
        int safety =5000; // 세이프가드
        while (openSet.Count > 0 && safety-- > 0)
        {
            // openSet에서 fCost가 가장 낮은 노드를 선택
            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            // 현재 노드 처리
            openSet.Remove(currentNode);
            // 노드 위치를 닫힌 집합에 추가응
            closedSet.Add(currentNode.position);
            // 목표 지점에 착창, 도착 했으면 경로 반환
            if (currentNode.position == goal){ return RetracePath(currentNode);}

            // 이웃 노드 탐색 (4방형 : 상ㅡ하ㅡ좌ㅡ우)
            foreach (var neighbourPos in GetNeighbours(currentNode.position))
            {
                if (closedSet.Contains(neighbourPos) || blocked.Contains(neighbourPos))
                    continue; // 이미 방문했거나, 막힌 타일이면 무시


                int newGCost = currentNode.gCost + 1; // 모든 이동 비용이 1이라고 가정
                if (!allNodes.ContainsKey(neighbourPos))
                {
                    // 새로운 노드 생성
                    Node neighbour = new Node(neighbourPos);
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetHeurustic(neighbourPos, goal);
                    neighbour.parent = currentNode;
                    allNodes[neighbourPos] = neighbour;
                    openSet.Add(neighbour);
                    // 새로운 노드 발견
                }
                else
                {
                    // 기존 노드인데 더 짧은 경로 발견 -> 갱신
                    Node neighbour = allNodes[neighbourPos];
                    if (newGCost < neighbour.gCost)
                    {
                        neighbour.gCost = newGCost;
                        neighbour.parent = currentNode;
                    }
                }
            }
        }
        return new List<Vector3Int>(); // 경로 없음
    }

    // 목표 노드에서 부모를 따라가며 경로 역추적

    // endNode
    private List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();  // 시작 -> 목표 순서로 정렬
        path.RemoveAt(0); // 첫 칸(시작 지점)은 빼도 됨
        return path;
    }

    // 휴리스틱 (맨해튼 거리 사용)
    //a
    //b
    private int GetHeurustic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);// 맨해튼 거리
    }
    // 안녕 하세요 저는 장보석 이라고 합니다

    //현재 좌표에서 4방향 이웃 좌표 반환
    //nodePos
    private IEnumerable<Vector3Int> GetNeighbours(Vector3Int nodePos)
    {
        if ((nodePos.x >= 0 && nodePos.x < 10) && (nodePos.y >= 0 && nodePos.y < 10))
        {
            yield return new Vector3Int(nodePos.x + 1, nodePos.y, nodePos.z);
            yield return new Vector3Int(nodePos.x - 1, nodePos.y, nodePos.z);
            yield return new Vector3Int(nodePos.x, nodePos.y + 1, nodePos.z);
            yield return new Vector3Int(nodePos.x, nodePos.y - 1, nodePos.z);
        }
    }
}