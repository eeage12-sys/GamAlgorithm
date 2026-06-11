using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform target;          // 플레이어 (Player)

    [Header("거리 및 높이 설정")]
    public float distance = 4.0f;
    public float height = 1.8f;

    // [핵심 추가] 캐릭터를 화면 왼쪽으로 치워주는 오프셋 값
    [Header("가로 위치 오프셋 (캐릭터 왼쪽으로 치우기)")]
    public float sideOffset = 1.5f;   // 값이 클수록 캐릭터가 더 왼쪽으로 갑니다.

    [Header("회전 속도")]
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    [Header("상하 회전 각도 제한")]
    public float yMinLimit = -5f;
    public float yMaxLimit = 50f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // [핵심 변경] 플레이어 몸통도 카메라의 좌우 회전에 맞춰 같이 돌려야 조준이 편해집니다.
            target.rotation = Quaternion.Euler(0, x, 0);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

            // [핵심 변경] 위치 계산 로직 수정
            // 1. 캐릭터 머리/어깨 높이 구하기
            Vector3 targetPosition = target.position + new Vector3(0, height, 0);

            // 2. 캐릭터를 화면 왼쪽으로 치워주는 Side Offset 계산 (카메라 회전에 맞춘 로컬 오른쪽 방향)
            Vector3 offset = rotation * Vector3.right * sideOffset;

            // 3. 최종 위치: (회전 계산된 위치) + (Side Offset)
            Vector3 position = rotation * negDistance + targetPosition + offset;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}