using UnityEngine;

public class MonsterStateGizmo : MonoBehaviour
{
    private enum MonsterState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Target")]
    [Tooltip("몬스터가 감지하고 추적할 플레이어 Transform입니다.")]
    [SerializeField] private Transform player;

    [Header("Distance")]
    [Tooltip("이 거리 안으로 플레이어가 들어오면 Chase 상태로 전환합니다.")]
    [SerializeField] private float chaseDistance = 6f;

    [Tooltip("이 거리 안으로 플레이어가 들어오면 Attack 상태로 전환합니다.")]
    [SerializeField] private float attackDistance = 1.8f;

    private MonsterState currentState = MonsterState.Idle;

    private void Update()
    {
        if (player == null)
        {
            currentState = MonsterState.Idle;
            return;
        }

        // Vector3.Distance는 두 위치 사이의 거리를 계산합니다.
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            currentState = MonsterState.Attack;
        }
        else if (distance <= chaseDistance)
        {
            currentState = MonsterState.Chase;
        }
        else
        {
            currentState = MonsterState.Idle;
        }
    }

    private void OnDrawGizmos()
    {
        // OnDrawGizmos는 Scene 뷰에 개발용 시각 표시를 그릴 때 사용하는 Unity 메시지 메서드입니다.
        Gizmos.color = GetStateColor();
        Gizmos.DrawSphere(transform.position, 0.35f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (player == null)
        {
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, player.position);
    }

    private Color GetStateColor()
    {
        switch (currentState)
        {
            case MonsterState.Chase:
                return Color.yellow;
            case MonsterState.Attack:
                return Color.red;
            default:
                return Color.gray;
        }
    }
}