using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    [Header("UI ¿¬µ¿")]
    [SerializeField] private Slider hpSlider;

    private Transform mainCameraTransform;

    void Start()
    {
        
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        
        if (mainCameraTransform != null)
        {
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
    }

    
    public void UpdateHPBar(float currentHP, float maxHP)
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHP / maxHP;
        }
    }
}