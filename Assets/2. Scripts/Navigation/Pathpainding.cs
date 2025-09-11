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
        // TODO: �ʿ��ϸ� Ÿ�ϸʿ��� ��Ÿ���� �о�ͼ� bblocked�� �߰�����
    }
    /// <summary>
    /// A * �˰������� start -> goal ������ ��θ� ����
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        //Ž�� �ĺ��� (openSet) �� �̹� �湮�� ���� (closedSet)
        List<Node> openSet = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // ���� ��� �ʱ�ȭ(���)
        Node startNode = new Node(start);
        openSet.Add(startNode);

        //��ü ��带 ����(���� ��ǥ�� ��带 �����ϱ� ����)
        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

        allNodes[start] = startNode;
        while (openSet.Count > 0)
        {
            // openSet���� fCost�� ���� ���� ��带 ����
            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            // ���� ��� ó��
            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            // ��ǥ ������ ��â, ���� ������ ��� ��ȯ
            if (currentNode.Position == goal)
            {
                return RetracePath(currentNode);
            }
            // �̿� ��� Ž�� (4���� : ����Ϥ��¤ѿ�)
            foreach (var neighbourPos in GetNeighbours(currentNode.Position))
            {
                if (closedSet.Contains(neighbourPos) || blocked.Contains(neighbourPos))
                    continue; // �̹� �湮�߰ų�, ���� Ÿ���̸� ����
                
                int newGCost = currentNode.gCost + 1; // ��� �̵� ����� 1�̶�� ����
                if (!allNodes.ContainsKey(neighbourPos))
                {
                    // ���ο� ��� ����
                    Node neighbour = new Node(neighbourPos);
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetHeurustic(neighbourPos, goal);
                    neighbour.Parent = currentNode;
                    allNodes[neighbourPos] = neighbour;
                    openSet.Add(neighbour);
                    // ���ο� ��� �߰�
                }
                else
                {
                    // ���� ����ε� �� ª�� ��� �߰� -> ����
                    Node neighbour = allNodes[neighbourPos];
                    if (newGCost < neighbour.gCost)
                    {
                        neighbour.gCost = newGCost;
                        neighbour.Parent = currentNode;
                    }
                }
            }
        }
        return new List<Vector3Int>(); // ��� ����
    }
    /// <summary>
    /// ��ǥ ��忡�� �θ� ���󰡸� ��� ������
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
        path.Reverse();  // ���� -> ��ǥ ������ ����
        path.RemoveAt(0); // ù ĭ(���� ����)�� ���� ��
        return path;
    }
    /// <summary>
    /// �޸���ƽ (����ư �Ÿ� ���)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int GetHeurustic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);// ����ư �Ÿ�
    }
    /// <summary>
    /// ���� ��ǥ���� 4���� �̿� ��ǥ ��ȯ
    /// </summary>
    /// <param name="nodePos"></param>
    /// <returns></returns>
    private IEnumerable<Vector3Int> GetNeighbours(Vector3Int nodePos)
    {
        yield return new Vector3Int(nodePos.x + 1, nodePos.y, nodePos.z);
        yield return new Vector3Int(nodePos.x - 1, nodePos.y, nodePos.z);
        yield return new Vector3Int(nodePos.x, nodePos.y + 1, nodePos.z);
        yield return new Vector3Int(nodePos.x, nodePos.y - 1, nodePos.z);
        // �밢�� �̵� �߰��Ϸ��� ���⿡ �߰�
    }
}