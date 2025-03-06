using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //컨트롤 움직임 
    [Header("Movement")]
    public float moveSpeed;
    public float _jumpPower;
    private Vector2 _curMovementInput;
    public LayerMask groundLayMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rd;

    private void Awake()
    {
        _rd = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FixedUpdate()
    {
        if (canLook)
        {
            Move();
        }
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            CamerLook();
        }
    }
    void Move()
    {
        //백터의 곱을 이용해서 방향을 정하기 
        // x축 *z축 = (transform.right *_curMovementInput.x) + (transform.forward *_curMovementInput.y)
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        //방향(백터) *속도(스칼라)
        dir *= moveSpeed;
        // 중력 
        dir.y = _rd.velocity.y;

        // 다시 넣어주면 그 방향으로 간다.
        _rd.velocity = dir;
    }

    void CamerLook()
    {
        // 마우스 움직임의 변화량 중 y 위 아래 갑셍 민감도를 곱한다)
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnLock(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGround())
        {
            _rd.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGround()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position+  (transform.forward*0.2f)+(transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f),Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f),Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f),Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayMask))
            {
                return true;
            }
        }

        return false;
    }


    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
