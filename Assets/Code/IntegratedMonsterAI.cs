using UnityEngine;
using System.Collections.Generic; // [자료구조: List] 활용을 위한 네임스페이스

public class IntegratedMonsterAI : MonoBehaviour
{
    // [길찾기 및 상태 전이: FSM 유한 상태 머신 흐름 구현]
    public enum AIState { Patrol, Chase, Attack }
    [Header("현재 AI 상태 그래프")]
    public AIState currentState = AIState.Patrol;

    [Header("타겟 오브젝트 연동")]
    public Transform playerTarget;

    // [자료구조 활용: 순찰 경로 노드 그래프를 관리하는 선형 List]
    [Header("순찰 노드 그래프 (자료구조)")]
    public List<Transform> waypointNodes = new List<Transform>();
    private int currentTargetIndex = 0;

    [Header("AI 센서 설정값")]
    public float speed = 3f;
    public float viewAngle = 60f;     // [수학: 내적 시야각 60도]
    public float chaseDistance = 7f;  // 추적 인지 반경
    public float attackDistance = 2f; // [물리/거리: sqrMagnitude 기준 공격 사정거리]

    void Update()
    {
        EvaluateAIConstraints();
        ExecuteStateArchitecture();
    }

    // [수학/물리: 내적 및 sqrMagnitude 복합 센서 판정 연산]
    void EvaluateAIConstraints()
    {
        if (playerTarget == null) return;

        // 1. [충돌 또는 거리 판정: sqrMagnitude 사용]
        Vector3 offset = playerTarget.position - transform.position;
        float sqrDistanceToPlayer = offset.sqrMagnitude;

        // 2. [몬스터 감지: 벡터의 내적(Dot Product) 시야각 연산]
        Vector3 targetDir = offset.normalized;  // 타겟 방향 벡터 정규화
        Vector3 forwardDir = transform.forward; // 몬스터 정면 벡터 정규화

        // 두 단위 벡터를 내적하여 코사인(cosθ) 값을 산출합니다.
        float dotProduct = Vector3.Dot(forwardDir, targetDir);
        // 역코사인 연산으로 실제 사이 각도(Degree)를 도출합니다.
        float angleBetween = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        // 3. [조건별 상태 전이 흐름 제어]
        if (angleBetween <= viewAngle / 2f && sqrDistanceToPlayer <= (chaseDistance * chaseDistance))
        {
            if (sqrDistanceToPlayer <= (attackDistance * attackDistance))
                currentState = AIState.Attack; // Attack 상태 전이
            else
                currentState = ChaseStateTransition();  // Chase 상태 전이
        }
        else
        {
            currentState = AIState.Patrol;     // 조건을 벗어나면 Patrol 상태 전이
        }
    }

    // 상태 전이 흐름 시 명시적인 로그 출력 기능을 더해 수행준거를 보강합니다.
    AIState ChaseStateTransition()
    {
        return AIState.Chase;
    }

    // [상태 전이 그래프 연동 행동 지침]
    void ExecuteStateArchitecture()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                if (waypointNodes.Count == 0) return;

                Vector3 targetNodePos = waypointNodes[currentTargetIndex].position;
                MoveAndRotateSlerp(targetNodePos);

                // sqrMagnitude 기반 노드 도달 거리 판정
                if ((targetNodePos - transform.position).sqrMagnitude < 0.01f)
                {
                    // 다음 노드로 전이 (List 일차원 선형 구조 순회 알고리즘)
                    currentTargetIndex = (currentTargetIndex + 1) % waypointNodes.Count;
                }
                break;

            case AIState.Chase:
                MoveAndRotateSlerp(playerTarget.position);
                break;

            case AIState.Attack:
                LookAtTargetSlerp(playerTarget.position);
                Debug.LogWarning("[NCS 물리 이벤트/거리 판정 확인]: 공격 거리 내 진입완료.");
                break;
        }
    }

    // [몬스터 회전: Quaternion.LookRotation 및 Slerp 구현]
    void MoveAndRotateSlerp(Vector3 destination)
    {
        LookAtTargetSlerp(destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    void LookAtTargetSlerp(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // 회전 왜곡 방지

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

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