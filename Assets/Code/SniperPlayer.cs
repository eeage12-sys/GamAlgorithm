using UnityEngine;

public class SniperPlayer : MonoBehaviour
{
    public float baseDamage = 20f;
    public float maxDistance = 100f;
    public LayerMask monsterLayer; // 몬스터만 골라 조준할 레이어마스크

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭 시 발사
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 내(Player) 위치에서 앞방향(몬스터 쪽)으로 레이저를 생성
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // monsterLayer에 해당하는 오브젝트만 충돌을 감지합니다! (장애물 무시 최적화)
        if (Physics.Raycast(ray, out hit, maxDistance, monsterLayer))
        {
            string hitTag = hit.collider.tag;

            // 맞은 부위의 부모(Monster)에게 있는 대본(스크립트)을 가져옴
            MonsterController monster = hit.collider.GetComponentInParent<MonsterController>();

            if (monster != null)
            {
                if (hitTag == "MonsterHead") monster.TakeDamage(baseDamage, "Head");
                else if (hitTag == "MonsterLeg") monster.TakeDamage(baseDamage, "Leg");
                else if (hitTag == "MonsterBody") monster.TakeDamage(baseDamage, "Body");
            }
        }
        // 개발자 화면에 노란색 조준선 그리기
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 0.5f);
    }
}