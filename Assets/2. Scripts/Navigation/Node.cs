using UnityEngine;
public class Node
{
    public Vector3Int Position; // ���� ��� ĭ�� �ִ���
    public int gCost; // ���ݱ��� �ɾ�� �Ÿ�
    public int hCost; // ��ǥ������ �Ÿ���
    public int FCost => gCost + hCost; // �� ���
    public Node Parent; // ���� ��� �Դ���
    public Node(Vector3Int position)
    {
        Position = position;
    }
}