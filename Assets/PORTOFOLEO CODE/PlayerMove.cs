using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private bool isGrounded = true;
    private Camera mainCamera;

    // 내부적으로 사용할 레이저 발사 시작점과 길이 계산용 변수
    private Vector3 rayStartOffset;
    private float strictCheckDistance = 0.3f; // 발바닥 밑으로 30cm만 검사

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        if (rb != null)
        {
            // Y축 회전은 코드에서 제어하므로 물리 엔진에 의한 X, Z축 회전만 고정
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // 시작할 때 콜라이더 크기를 기반으로 발밑 레이저 시작 위치를 미리 계산합니다.
        CalculateRaycastOffset();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // ★ 매 프레임 발바닥 바로 밑을 정밀 체크합니다.
        CheckGroundedWithRaycast();

        // 점프 체크 (스페이스바) - 이제 평지와 큐브 계단 위 모두 완벽하게 작동합니다.
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            if (rb != null)
            {
                // 순간적인 힘을 위로 가함
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

                // 물리 엔진이 캐릭터를 억지로 회전시키려는 속도를 매 순간 '0'으로 만듭니다.
                rb.angularVelocity = Vector3.zero;
            }

            // 2. 캐릭터가 이동하는 방향을 코드가 계산해서 부드럽게 회전시킵니다.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // 콜라이더 크기를 분석하여 '발바닥 살짝 위쪽'을 레이저 시작점으로 설정하는 함수
    void CalculateRaycastOffset()
    {
        if (playerCollider != null)
        {
            // 중심점 기준 캐릭터 높이의 절반 아래에서 0.1f(10cm) 위쪽을 시작점으로 잡습니다. (오차 방지)
            float yOffset = -(playerCollider.height / 2f) + playerCollider.center.y + 0.1f;
            rayStartOffset = new Vector3(0f, yOffset, 0f);
        }
        else
        {
            // 콜라이더가 없을 때의 최소한의 예외 처리 기본값
            rayStartOffset = new Vector3(0f, -0.9f, 0f);
        }
    }

    // 💥 [업그레이드] 발바닥 자동 추적형 레이캐스트 바닥 감지 시스템
    void CheckGroundedWithRaycast()
    {
        // 실시간 캐릭터 위치에 오프셋을 더해 정확한 발밑 시작점을 구합니다.
        Vector3 rayStartPos = transform.position + rayStartOffset;

        Ray ray = new Ray(rayStartPos, Vector3.down);
        RaycastHit hit;

        // 게임 뷰 및 씬 뷰에서 작동 상태를 눈으로 볼 수 있게 빨간색 레이저를 실시간으로 그립니다.
        Debug.DrawRay(rayStartPos, Vector3.down * strictCheckDistance, Color.red);

        // 발밑 30cm 안에 무언가 걸리는지 확인
        if (Physics.Raycast(ray, out hit, strictCheckDistance))
        {
            // 부딪힌 물체의 태그가 "Ground"(평지 및 큐브 계단)라면 바닥에 서 있는 것으로 인정!
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                return;
            }
        }

        // 공중에 떠 있는 상태
        isGrounded = false;
    }

    // 에디터 씬 뷰에서 플레이어를 클릭했을 때 레이저가 어디서 나가는지 보여주는 가이드라인
    void OnDrawGizmosSelected()
    {
        CalculateRaycastOffset();
        Gizmos.color = Color.red;
        Vector3 previewPos = transform.position + rayStartOffset;
        Gizmos.DrawLine(previewPos, previewPos + Vector3.down * strictCheckDistance);
    }
}