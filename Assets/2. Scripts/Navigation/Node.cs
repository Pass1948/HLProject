using UnityEngine;
public class Node
{
    public Vector3Int Position; // 지금 어디 칸에 있는지
    public int gCost; // 지금까지 걸어온 거리
    public int hCost; // 목표까지의 거리이
    public int FCost => gCost + hCost; // 총 비용
    public Node Parent; // 내가 어디서 왔는지
    public Node(Vector3Int position)
    {
        Position = position;
    }
}