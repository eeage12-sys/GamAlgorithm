using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // 메인 카메라의 Transform을 가져옵니다.
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (camTransform != null)
        {
            // Canvas가 항상 카메라를 정면으로 바라보도록 설정
            transform.LookAt(transform.position + camTransform.forward);
        }
    }
}