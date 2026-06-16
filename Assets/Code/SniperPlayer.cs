using UnityEngine;

public class SniperPlayer : MonoBehaviour
{
    public float baseDamage = 20f;
    public float maxDistance = 100f;
    public LayerMask monsterLayer; 

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, monsterLayer))
        {
            string hitTag = hit.collider.tag;

            MonsterController monster = hit.collider.GetComponentInParent<MonsterController>();

            if (monster != null)
            {
                if (hitTag == "MonsterHead") monster.TakeDamage(baseDamage, "Head");
                else if (hitTag == "MonsterLeg") monster.TakeDamage(baseDamage, "Leg");
                else if (hitTag == "MonsterBody") monster.TakeDamage(baseDamage, "Body");
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 0.5f);
    }
}