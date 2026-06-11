using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    [Header("UI 연동")]
    [SerializeField] private Slider hpSlider;

    private Transform mainCameraTransform;

    void Start()
    {
        // 항상 카메라를 바라보게 하기 위해 메인 카메라 컴포넌트 캐싱
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        // UI가 항상 카메라를 정면으로 바라보도록 회전 값 고정 (빌보드 기능)
        if (mainCameraTransform != null)
        {
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
    }

    // AI 스크립트에서 체력이 바뀔 때마다 이 메서드를 호출해 줄 겁니다.
    public void UpdateHPBar(float currentHP, float maxHP)
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHP / maxHP;
        }
    }
}