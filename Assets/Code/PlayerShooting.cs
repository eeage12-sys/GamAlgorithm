using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f; // 속도를 힘차게 올려두었습니다!

    private Camera mainCamera;

    void Start()
    {
        // 화면을 비추고 있는 메인 카메라를 자동으로 가져옵니다.
        mainCamera = Camera.main;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            ShootTowardMouse(mouse.position.ReadValue());
        }
    }

    void ShootTowardMouse(Vector2 mousePosition)
    {
        if (bulletPrefab == null || firePoint == null || mainCamera == null) return;

        // 1. 마우스로 클릭한 화면 좌표에서 3D 세상으로 뻗어나가는 '화이지(Ray)'를 만듭니다.
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // 기본 발사 방향은 일단 플레이어의 정면으로 세팅해 둡니다.
        Vector3 targetDirection = firePoint.forward;

        // 2. 만약 마우스 레이저가 몬스터의 부위(머리, 몸, 다리 등)나 무언가에 부딪혔다면!
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // 부딪힌 정확한 3D 좌표(hit.point)에서 총구 위치(firePoint.position)를 빼서 정확한 조준 방향을 계산합니다.
            targetDirection = (hit.point - firePoint.position).normalized;
        }

        // 3. 총알이 조준 방향을 올바르게 바라보도록 회전값을 계산하고 90도 눕혀줍니다.
        Quaternion bulletRotation = Quaternion.LookRotation(targetDirection);
        bulletRotation *= Quaternion.Euler(90f, 0f, 0f);

        // 4. 총알 생성
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        // 5. 계산된 정확한 조준 방향(targetDirection)으로 속도를 줍니다!
        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = targetDirection * bulletSpeed;
        }

        Destroy(bulletInstance, 3f);
    }
}