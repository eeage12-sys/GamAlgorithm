using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 점프 체크 (스페이스바)
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            if (rb != null)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }


    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector3 inputDirection = Vector3.zero;

        if (keyboard.wKey.isPressed) inputDirection += Vector3.forward;
        if (keyboard.sKey.isPressed) inputDirection += Vector3.back;
        if (keyboard.aKey.isPressed) inputDirection += Vector3.left;
        if (keyboard.dKey.isPressed) inputDirection += Vector3.right;

        inputDirection = inputDirection.normalized;

        if (inputDirection != Vector3.zero && mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward = camForward.normalized;
            camRight = camRight.normalized;

            Vector3 moveDirection = (camForward * inputDirection.z + camRight * inputDirection.x).normalized;

            if (rb != null)
            {
                // 1. 물리 이동 처리
                rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

                // ★[여기에 이 코드를 추가합니다!]★
                // 물리 엔진이 캐릭터를 억지로 회전시키려는 속도(Angular Velocity)를 매 순간 '0'으로 만듭니다.
                rb.angularVelocity = Vector3.zero;
            }

            // 2. 캐릭터가 이동하는 방향을 코드가 계산해서 부드럽게 회전시킵니다.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // 바닥 충돌 체크 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
} // <--- 이 마지막 괄호가 전체 class를 닫아주는 가장 중요한 괄호입니다!