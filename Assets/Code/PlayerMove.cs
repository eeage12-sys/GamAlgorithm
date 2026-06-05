using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb; // 물리 컴포넌트를 담을 그릇

    void Start()
    {
        // 게임이 시작될 때 오브젝트에 붙어있는 Rigidbody를 자동으로 가져옵니다.
        rb = GetComponent<Rigidbody>();

        // [꿀팁] 코드로도 넘어짐 방지를 확실하게 쐐기 박아줄 수 있습니다.
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    // 물리 이동은 Update가 아니라 FixedUpdate에서 처리하는 것이 유니티 6 물리 공식 규칙입니다!
    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        Vector3 moveDirection = Vector3.zero;

        if (keyboard.wKey.isPressed) moveDirection += Vector3.forward;
        if (keyboard.sKey.isPressed) moveDirection += Vector3.back;
        if (keyboard.aKey.isPressed) moveDirection += Vector3.left;
        if (keyboard.dKey.isPressed) moveDirection += Vector3.right;

        // [필수구현: 대각선 이동 속도 보정 (정규화)]
        moveDirection = moveDirection.normalized;

        // [변경점]: transform.position 대신 물리 엔진(rb)으로 이동시킵니다.
        // 이렇게 해야 벽이나 몬스터에 부딪혔을 때 뚫지 못하고 딱 멈춰 섭니다!
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
}