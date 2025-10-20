using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Transform followTarget;
    [SerializeField] private float boundHalfX = 2f;     // 포인터 기준 좌우 허용경계
    [SerializeField] private float boundHalfZ = 2f;     // 포인터 기준 앞뒤 허용 경계
    private Vector3 player;
    private float moveSpeed = 10f;
    private float zoomSpeed = 20f;
    private float minZoom = 20f;
    private float maxZoom = 60f;
    private float edgeSize = 20f;
    private float recentSpeed = 5f;

    private Camera cam;
    private bool recentering = false;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.CameraSenter, OnSenter);
    }
    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.CameraSenter, OnSenter);
    }

    public void InitCamera()
    {
        cam = Camera.main;
        cam.transform.rotation = Quaternion.Euler(60f, 45f, 0f);
        Vector3 playerPos = new Vector3(GameManager.Map.GetPlayer2Position().x, GameManager.Map.GetPlayer2Position().y, 0);
        player = playerPos;
    }

    void Update()
    {
        if (followTarget == null && GameManager.Mouse != null && GameManager.Mouse.pointer != null)
            followTarget = GameManager.Mouse.pointer;
        if (!GameManager.TurnBased.isCamera) return;
        HandleZoom();
        if (!recentering) HandleEdgeScroll();
        HandleRecenter();
    }

    private void HandleZoom()
    {
        if (cam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }

    private void HandleEdgeScroll()
    {
        if (cam == null) return;

        Vector3 moveDir = Vector3.zero;
        Vector2 mousePos = Input.mousePosition;
        float edge = edgeSize;


        if (mousePos.x <= edge) moveDir.x = -1;
        else if (mousePos.x >= Screen.width - edge) moveDir.x = 1;

        if (mousePos.y <= edge) moveDir.z = -1;
        else if (mousePos.y >= Screen.height - edge) moveDir.z = 1;
        if (moveDir == Vector3.zero) return;

        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 desiredMove = (forward * moveDir.z + right * moveDir.x).normalized;

        Vector3 cur = cam.transform.position;
        Vector3 cand = cur + desiredMove * moveSpeed * Time.deltaTime;
        if (followTarget != null)
        {
            Vector3 t = followTarget.position;
            float minX = t.x - boundHalfX;
            float maxX = t.x + boundHalfX;
            float minZ = t.z - boundHalfZ;
            float maxZ = t.z + boundHalfZ;

            if (cur.x < minX)
            {
                if (cand.x < cur.x) 
                    cand.x = cur.x;
            }
            else if (cur.x > maxX)
            {
                if (cand.x > cur.x)
                    cand.x = cur.x;
            }
            else
            {
                cand.x = Mathf.Clamp(cand.x, minX, maxX);
            }

            if (cur.z < minZ)
            {
                if (cand.z < cur.z) 
                    cand.z = cur.z;
            }
            else if (cur.z > maxZ)
            {
                if (cand.z > cur.z) cand.z = cur.z;
            }
            else
            {
                cand.z = Mathf.Clamp(cand.z, minZ, maxZ);
            }
        }

        cam.transform.position = cand;
    }

    private void HandleRecenter()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            recentering = true;

        if (recentering)
        {
            Vector3 targetPos = player;
            targetPos.y = cam.transform.position.y;

            cam.transform.position = Vector3.Lerp(
                cam.transform.position,
                targetPos,
                Time.deltaTime * recentSpeed);

            if (Vector3.Distance(new Vector3(cam.transform.position.x, 0, cam.transform.position.z),
                    new Vector3(player.x, 0, player.z)) < 0.1f)
                recentering = false;
        }
    }

    private void OnSenter()
    {
        recentering = true;
    }
}
