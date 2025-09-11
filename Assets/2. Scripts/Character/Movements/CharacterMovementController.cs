using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class CharacterMovementController : MonoBehaviour 
{
    public enum GridPlane {XY,XZ }
    [Header("Grid Settings")]
    [SerializeField] private Tilemap tilemap; // �̵� ���� Ÿ�ϸ�
    [SerializeField] private GridPlane gridPlane = GridPlane.XZ; // �׸��� ��� ����
    [SerializeField] private float groundY = 0f; // �׸��� �� ũ��

    [Header("Movement Settings")]
    [SerializeField] private float moveTime = 0.2f;

    private Vector3Int _cellPosition; // ���� ĳ���Ͱ� �ִ� Ÿ�� ��ǥ
    private bool _isMoving = false;  // �̵� �� ����

    private Pathfinding _pathfinding;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        // ���� �� ĳ���͸� Ÿ�� �߽����� ����
        _cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_cellPosition);

        // ��� Ž���� �ʱ�ȭ
        _pathfinding = new Pathfinding(tilemap);

        // ���� ������ �ʱ�ȭ...
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.widthMultiplier = 0.1f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.green;

    }
    private void Update()
    {
        if(TryGetMouseWorldOnGrid(out var mouseWorld))
        {
            var targetCell = tilemap.WorldToCell(mouseWorld);

            if(targetCell != _cellPosition)
            {
                var path = _pathfinding.FindPath(_cellPosition, targetCell);
                Debug.Log($"Path Count: {path.Count}");
                DrowPath(path);
            }
            else
            {
                _lineRenderer.positionCount = 0;
            }
        }
    }

    private void DrowPath(List<Vector3Int> path)
    {
        if (path.Count < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }
        _lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            _lineRenderer.SetPosition(i, tilemap.GetCellCenterWorld(path[i]) + Vector3.up * 0.1f);
        }
    }


    private void OnMovementClick(InputValue value)
    {
        // UI �� Ŭ���� ����
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // ��ư �׼��̹Ƿ� pressed�� �ƴϸ� ����
        if (!value.isPressed) return;

        // ���콺�� ����Ű�� '�׸��� ���' ���� ���� ��ǥ�� ���Ѵ�.
        if (!TryGetMouseWorldOnGrid(out var mouseWorld)) return;

        // �� ���� ��ǥ�� �� ��ǥ�� ��ȯ
        var targetCell = tilemap.WorldToCell(mouseWorld);

        // ���� ĭ�̸� �� �� ����
        if (targetCell == _cellPosition) return;

        // A* ��� ã��
        List<Vector3Int> path = _pathfinding.FindPath(_cellPosition, targetCell);

        if (path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
        }
        _lineRenderer.positionCount = 0;
        
    }

    /// <summary>
    /// ī�޶� ����/�׸��� ��鿡 ���� ���콺�� ���� '�׸��� ���'���� ���� ��ǥ�� ��´�.
    /// </summary>
    private bool TryGetMouseWorldOnGrid(out Vector3 world)
    {
        var mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

        //XZ ���
        Plane plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            world = ray.GetPoint(enter);
            return true;
        }
        world = default;
        return false;
    }

    private IEnumerator FollowPath(List<Vector3Int> path)
    {
        foreach (var cell in path)
            yield return MoveRoutine(cell);
    }

    private IEnumerator MoveRoutine(Vector3Int targetCell)
    {
        _isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = tilemap.GetCellCenterWorld(targetCell);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        _cellPosition = targetCell;
        _isMoving = false;
    }
}