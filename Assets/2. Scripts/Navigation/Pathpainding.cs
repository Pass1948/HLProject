using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Pathfinding
{
    private Tilemap tilemap;
    private HashSet<Vector3Int> blocked;
    public Pathfinding(Tilemap tilemap)
    {
        this.tilemap = tilemap;
        blocked = new HashSet<Vector3Int>();
        // TODO: 필요하면 타일맵에서 벽타일을 읽어와서 bblocked에 추가해응
    }
    /// <summary>
    /// A * 알고리즘으로 start -> goal 까지의 경로를 구함
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        //탐색 후보군 (openSet) 과 이미 방문한 집합 (closedSet)
        List<Node> openSet = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // 시작 노드 초기화(등록)
        Node startNode = new Node(start);
        openSet.Add(startNode);

        //전체 노드를 저장(같은 좌표의 노드를 재사용하기 위함)
        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

        allNodes[start] = startNode;
        while (openSet.Count > 0)
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
            closedSet.Add(currentNode.Position);

            // 목표 지점에 착창, 도착 했으면 경로 반환
            if (currentNode.Position == goal)
            {
                return RetracePath(currentNode);
            }
            // 이웃 노드 탐색 (4방형 : 상ㅡ하ㅡ좌ㅡ우)
            foreach (var neighbourPos in GetNeighbours(currentNode.Position))
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
                    neighbour.Parent = currentNode;
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
                        neighbour.Parent = currentNode;
                    }
                }
            }
        }
        return new List<Vector3Int>(); // 경로 없음
    }
    /// <summary>
    /// 목표 노드에서 부모를 따라가며 경로 역추적
    /// </summary>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        path.Reverse();  // 시작 -> 목표 순서로 정렬
        path.RemoveAt(0); // 첫 칸(시작 지점)은 빼도 됨
        return path;
    }
    /// <summary>
    /// 휴리스틱 (맨해튼 거리 사용)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int GetHeurustic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);// 맨해튼 거리
    }
    /// <summary>
    /// 현재 좌표에서 4방향 이웃 좌표 반환
    /// </summary>
    /// <param name="nodePos"></param>
    /// <returns></returns>
    private IEnumerable<Vector3Int> GetNeighbours(Vector3Int nodePos)
    {
        yield return new Vector3Int(nodePos.x + 1, nodePos.y, nodePos.z);
        yield return new Vector3Int(nodePos.x - 1, nodePos.y, nodePos.z);
        yield return new Vector3Int(nodePos.x, nodePos.y + 1, nodePos.z);
        yield return new Vector3Int(nodePos.x, nodePos.y - 1, nodePos.z);
        // 대각선 이동 추가하려면 여기에 추가
    }
}