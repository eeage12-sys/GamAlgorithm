using UnityEngine;
using UnityEngine.InputSystem; // 최신 인풋 시스템 네임스페이스

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Update는 매 프레임마다 자동 호출되는 유니티 생명주기 메서드입니다.
    void Update()
    {
        // 1. 사용자 입력 받기 (New Input System - Direct Read 방식)
        // Vector2.zero는 (0, 0) 벡터를 뜻하며, 입력이 없을 때의 기본값으로 사용합니다.
        Vector2 inputVector = Vector2.zero;

        // 키보드 장치가 연결되어 있는지 확인 후 입력값 계산 (현대적인 is not null 사용)
        // Keyboard.current는 현재 연결된 키보드 장치를 가져오는 프로퍼티입니다.
        if (Keyboard.current is not null)
        {
            float h = 0;
            float v = 0;

            // isPressed는 해당 키를 누르고 있는 동안 true가 되는 프로퍼티입니다.
            if (Keyboard.current.aKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed) h = 1;
            if (Keyboard.current.wKey.isPressed) v = 1;
            if (Keyboard.current.sKey.isPressed) v = -1;

            inputVector = new Vector2(h, v);
        }

        // 2. 방향 벡터 만들기 (입력은 2D 평면이지만 이동은 3D 공간의 X, Z축)
        // normalized는 벡터의 방향은 유지하고 길이를 1로 맞춘 값을 돌려주는 프로퍼티입니다.
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        // 3. 실제 이동 처리 (벡터 * 스칼라(속도) * 시간)
        // magnitude는 벡터의 길이를 뜻하며, 여기서는 이동 입력이 있는지 확인하는 데 사용합니다.
        if (moveDir.magnitude > 0)
        {
            // transform.Translate는 현재 오브젝트를 지정한 방향과 거리만큼 이동시키는 메서드입니다.
            // Time.deltaTime은 직전 프레임부터 현재 프레임까지 걸린 시간입니다.
            // Space.World는 이동 방향을 월드 좌표 기준으로 해석하겠다는 옵션입니다.
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}