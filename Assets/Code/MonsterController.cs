using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float hp = 100f;
    public float moveSpeed = 5f;

    public void TakeDamage(float damage, string part)
    {
        if (part == "Head")
        {
            float finalDamage = damage * 2f; // 머리는 2배 데미지
            hp -= finalDamage;
            Debug.Log($"[헤드샷!] 몬스터 머리에 {finalDamage} 데미지! 남은 체력: {hp}");
        }
        else if (part == "Leg")
        {
            hp -= damage;
            moveSpeed = 2f; // 다리는 이동속도 저하
            Debug.Log($"[부위 파괴!] 몬스터 다리에 {damage} 데미지! 속도 감소: {moveSpeed}, 남은 체력: {hp}");
        }
        else
        {
            hp -= damage;
            Debug.Log($"[몸통 적중] 몬스터 몸통에 {damage} 데미지! 남은 체력: {hp}");
        }

        if (hp <= 0) Destroy(gameObject); // 체력 0이면 사망
    }
}