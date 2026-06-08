using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;             // 따라다닐 플레이어
    public Vector3 offset = new Vector3(0f, 3.5f, -5f); // 플레이어 등 뒤로 떨어질 거리 (X:좌우, Y:높이, Z:뒤쪽거리)
    public float smoothSpeed = 8f;       // 카메라가 쫓아가는 부드러운 속도

    void LateUpdate()
    {
        if (target == null) return;

        // 플레이어의 위치에 offset을 더해 카메라의 목표 위치를 계산합니다.
        Vector3 targetPosition = target.position + offset;

        // 부드럽게 위치 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 카메라가 항상 플레이어 머리 살짝 위나 중심을 바라보게 합니다.
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}