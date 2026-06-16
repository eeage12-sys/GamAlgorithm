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

        if (mouse.leftButton.wasPressedThisFrame)
        {
            ShootTowardCenter(); 
        }
    }

    void ShootTowardCenter()
    {
        if (bulletPrefab == null || firePoint == null || mainCamera == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        Vector3 targetPoint = ray.GetPoint(100f);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }

        Vector3 targetDirection = (targetPoint - firePoint.position).normalized;

        Quaternion bulletRotation = Quaternion.LookRotation(targetDirection);
        bulletRotation *= Quaternion.Euler(90f, 0f, 0f);

        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = targetDirection * bulletSpeed;
        }

        Destroy(bulletInstance, 3f);
    }
}