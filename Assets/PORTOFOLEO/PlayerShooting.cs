using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // 마우스 왼쪽 버튼을 눌렀을 때 발사합니다.
        if (mouse.leftButton.wasPressedThisFrame)
        {
            ShootTowardCenter(); // 화면 중앙을 향해 쏘도록 함수 변경
        }
    }

    void ShootTowardCenter()
    {
        if (bulletPrefab == null || firePoint == null || mainCamera == null) return;

        // 핵심 변경: 마우스 좌표 대신 '화면 정중앙' 좌표를 만듭니다.
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // 화면 정중앙에서 레이저(Ray)를 쏩니다.
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // 레이저가 아무것도 안 맞았을 때의 기본 방향 (카메라가 바라보는 정면 멀리)
        Vector3 targetPoint = ray.GetPoint(100f);

        // 만약 화면 중앙 레이저에 무언가(벽, 땅, 몬스터)가 부딪혔다면
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // 그 부딪힌 지점을 목표 좌표로 설정합니다.
            targetPoint = hit.point;
        }

        // 총구(firePoint)에서 목표 지점(targetPoint)을 바라보는 정확한 방향을 계산합니다.
        Vector3 targetDirection = (targetPoint - firePoint.position).normalized;

        // 총알 회전값 계산 및 90도 눕히기
        Quaternion bulletRotation = Quaternion.LookRotation(targetDirection);
        bulletRotation *= Quaternion.Euler(90f, 0f, 0f);

        // 총알 생성
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        // 총알 날리기
        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 최신 유니티 버전에 맞게 속도 부여 (linearVelocity 또는 velocity)
            rb.linearVelocity = targetDirection * bulletSpeed;
        }

        Destroy(bulletInstance, 3f);
    }
}