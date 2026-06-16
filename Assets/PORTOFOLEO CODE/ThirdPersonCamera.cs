using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform target;         

    [Header("거리 및 높이 설정")]
    public float distance = 4.0f;
    public float height = 1.8f;

    
    [Header("가로 위치 오프셋 (캐릭터 왼쪽으로 치우기)")]
    public float sideOffset = 1.5f;   

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

            
            target.rotation = Quaternion.Euler(0, x, 0);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

            
            Vector3 targetPosition = target.position + new Vector3(0, height, 0);

            
            Vector3 offset = rotation * Vector3.right * sideOffset;

            
            Vector3 position = rotation * negDistance + targetPosition + offset;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}