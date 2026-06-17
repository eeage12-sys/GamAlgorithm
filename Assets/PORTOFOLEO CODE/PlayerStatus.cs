using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("플레이어 스탯 설정")]
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;

    
    [Header("UI 창 설정")]
    public GameObject statusWindow; // 상태창 (StatusWindow)
    public GameObject equipWindow;  // 장비창 (EquipWindow)

    void Start()
    {
        currentHealth = maxHealth;

        
        if (statusWindow != null) statusWindow.SetActive(false);
        if (equipWindow != null) equipWindow.SetActive(false);
    }

    
    void Update()
    {
        
        if (isDead) return;

        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (statusWindow != null)
            {
                bool isStatusOpen = !statusWindow.activeSelf;
                statusWindow.SetActive(isStatusOpen);
                Debug.Log($"[UI]: 상태창 열림 상태 = {isStatusOpen}");
            }
        }

        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (equipWindow != null)
            {
                bool isEquipOpen = !equipWindow.activeSelf;
                equipWindow.SetActive(isEquipOpen);
                Debug.Log($"[UI]: 장비창 열림 상태 = {isEquipOpen}");
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.LogWarning($"[플레이어 피격]: 몬스터에게 {damageAmount}의 대미지를 받았습니다! 남은 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.LogError("[게임 오버]: 플레이어가 사망했습니다.");

        
        if (statusWindow != null) statusWindow.SetActive(false);
        if (equipWindow != null) equipWindow.SetActive(false);

        if (GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;
        if (GetComponent<PlayerShooting>() != null) GetComponent<PlayerShooting>().enabled = false;
    }
}