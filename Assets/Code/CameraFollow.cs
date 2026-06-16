using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;             
    public Vector3 offset = new Vector3(0f, 3.5f, -5f); 
    public float smoothSpeed = 8f;       

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1f);
    }
}