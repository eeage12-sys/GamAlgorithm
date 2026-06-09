using UnityEngine;
using System.Collections.Generic;

public class IntegratedMonsterAI : MonoBehaviour
{
    // FSM 상태 머신
    public enum AIState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("현재 AI 상태 그래프")]
    public AIState currentState = AIState.Patrol;

    [Header("타겟 오브젝트 연동")]
    public Transform playerTarget;

    [Header("순찰 노드 그래프 (자료구조)")]
    public List<Transform> waypointNodes = new List<Transform>();

    private int currentTargetIndex = 0;

    [Header("AI 센서 설정값")]
    public float speed = 3f;
    public float viewAngle = 60f;
    public float chaseDistance = 7f;
    public float attackDistance = 2f;

    void Update()
    {
        EvaluateAIConstraints();
        ExecuteStateArchitecture();
    }

    // 플레이어 감지
    void EvaluateAIConstraints()
    {
        if (playerTarget == null) return;

        Vector3 offset = playerTarget.position - transform.position;
        float sqrDistanceToPlayer = offset.sqrMagnitude;

        Vector3 targetDir = offset.normalized;
        Vector3 forwardDir = transform.forward;

        float dotProduct = Vector3.Dot(forwardDir, targetDir);
        float angleBetween = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        if (angleBetween <= viewAngle / 2f && sqrDistanceToPlayer <= chaseDistance * chaseDistance)
        {
            if (sqrDistanceToPlayer <= attackDistance * attackDistance)
            {
                currentState = AIState.Attack;
            }
            else
            {
                currentState = AIState.Chase;
            }
        }
        else
        {
            currentState = AIState.Patrol;
        }
    }

    AIState ChaseStateTransition()
    {
        return AIState.Chase;
    }

    // 상태 실행
    void ExecuteStateArchitecture()
    {
        switch (currentState)
        {
            case AIState.Patrol:

                if (waypointNodes.Count == 0)
                    return;

                Vector3 targetNodePos = waypointNodes[currentTargetIndex].position;

                MoveAndRotateSlerp(targetNodePos);

                // ★ [버그 수정 완료] 
                // 기존 0.01f 방식은 너무 정밀해서 몬스터 덩치나 바닥 마찰 때문에 멈추면 인식을 못했습니다.
                // 오차 범위를 약 0.8m(제곱값 0.64f)로 널널하게 넓혀서, 노드 근처에만 가도 돌아가도록 수정했습니다.
                if ((targetNodePos - transform.position).sqrMagnitude < 0.64f)
                {
                    currentTargetIndex = (currentTargetIndex + 1) % waypointNodes.Count;
                }

                break;

            case AIState.Chase:

                Vector3 chaseTarget = playerTarget.position;

                // ★ 높이 고정
                chaseTarget.y = transform.position.y;

                MoveAndRotateSlerp(chaseTarget);

                break;

            case AIState.Attack:

                Vector3 attackTarget = playerTarget.position;

                // ★ 높이 고정
                attackTarget.y = transform.position.y;

                LookAtTargetSlerp(attackTarget);

                Debug.LogWarning("[NCS 물리 이벤트/거리 판정 확인]: 공격 거리 내 진입완료.");

                break;
        }
    }

    // 이동 + 회전
    void MoveAndRotateSlerp(Vector3 destination)
    {
        // ★ 목적지 높이 강제 고정
        destination.y = transform.position.y;

        LookAtTargetSlerp(destination);

        Vector3 nextPosition = Vector3.MoveTowards(
                transform.position,
                destination,
                speed * Time.deltaTime);

        // ★ 이동 후에도 높이 강제 고정
        nextPosition.y = transform.position.y;

        transform.position = nextPosition;
    }

    // 회전
    void LookAtTargetSlerp(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 5f);
        }
    }

    // Scene 뷰 시야각 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary * chaseDistance);
        Gizmos.DrawRay(transform.position, rightBoundary * chaseDistance);
    }
}