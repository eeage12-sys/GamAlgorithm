using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        Vector3 moveDirection = Vector3.zero;

        // W
        if (keyboard.wKey.isPressed)
        {
            moveDirection += Vector3.forward;
        }

        // S
        if (keyboard.sKey.isPressed)
        {
            moveDirection += Vector3.back;
        }

        // A
        if (keyboard.aKey.isPressed)
        {
            moveDirection += Vector3.left;
        }

        // D
        if (keyboard.dKey.isPressed)
        {
            moveDirection += Vector3.right;
        }

        // 대각선 이동 속도 보정
        moveDirection = moveDirection.normalized;

        transform.position +=
            moveDirection *
            moveSpeed *
            Time.deltaTime;
    }
}