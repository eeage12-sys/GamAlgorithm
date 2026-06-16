using UnityEngine;

public class MonsterSight : MonoBehaviour
{
    public Transform player;
    public float sightAngle = 60f;


    
    void Update()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        float dot = Vector3.Dot(transform.forward, dirToPlayer);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle < sightAngle)
        {
            Debug.Log("플레이어 발견!");

            Vector3 cross = Vector3 .Cross(transform.forward, dirToPlayer);

            if (cross.y > 0)
            {
                Debug.Log("플레이어는 오른쪽에 있음");
            }
            else if (cross.y < 0)
            {
                Debug.Log("플레이어는 왼쪽에 있음");
            }
        }
    }
}
