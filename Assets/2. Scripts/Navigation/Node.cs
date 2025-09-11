using UnityEngine;
public class Node
{
    public Vector3Int Position;
    public int gCost;
    public int hCost;
    public int FCost => gCost + hCost;
    public Node Parent;
    public Node(Vector3Int position)
    {
        Position = position;
    }
}