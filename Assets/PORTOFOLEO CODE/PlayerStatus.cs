using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("플레이어 스탯 설정")]
    public float maxHealth = 100f;   
    public float currentHealth;      
    private bool isDead = false;     

    void Start()
    {
        currentHealth = maxHealth;
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

        if (GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;
        if (GetComponent<PlayerShooting>() != null) GetComponent<PlayerShooting>().enabled = false;

    }
}