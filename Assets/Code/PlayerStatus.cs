using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("플레이어 스탯 설정")]
    public float maxHealth = 100f;   // 최대 체력
    public float currentHealth;      // 현재 체력
    private bool isDead = false;     // 사망 여부

    void Start()
    {
        // 시작할 때 체력 만땅으로 초기화
        currentHealth = maxHealth;
    }

    // 몬스터가 플레이어를 때릴 때 호출할 데미지 함수 (public 필수!)
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.LogWarning($"[플레이어 피격]: 몬스터에게 {damageAmount}의 대미지를 받았습니다! 남은 체력: {currentHealth}/{maxHealth}");

        // 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.LogError("[게임 오버]: 플레이어가 사망했습니다.");

        // 이동 및 슈팅 스크립트 비활성화해서 조작 불가능하게 만들기
        if (GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;
        if (GetComponent<PlayerShooting>() != null) GetComponent<PlayerShooting>().enabled = false;

        // 추가로 여기서 사망 애니메이션 재생 등을 처리할 수 있습니다.
    }
}