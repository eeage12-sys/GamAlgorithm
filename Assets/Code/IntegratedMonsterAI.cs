using UnityEngine;
using System.Collections.Generic;

public class IntegratedMonsterAI : MonoBehaviour
{
    // FSM 상태 머신
    public enum AIState
    {
        Patrol,
        Chase,
        Attack,
        Die // ⬅️ 사망 상태 추가
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

    [Tooltip("몸박 공격이 발동할 사거리입니다.")]
    public float attackDistance = 1.2f;

    [Header("몸박(Body Hit) 시스템 설정")]
    public int bodyHitDamage = 10;
    public float attackCooldown = 1.0f;
    private float currentAttackCooldown = 0f;

    [Header("몬스터 스탯 (체력 시스템)")]
    public float maxHealth = 100f;          // 최대 체력
    public float currentHealth;             // 현재 체력
    private bool isDead = false;            // 사망 플래그

    void Start()
    {
        // 게임 시작 시 체력 초기화
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return; // 죽었다면 모든 AI 로직 중지

        // 쿨타임 실시간 계산
        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        EvaluateAIConstraints();
        ExecuteStateArchitecture();
    }

    // 플레이어 감지 및 상태 전환 판정
    void EvaluateAIConstraints()
    {
        if (isDead || playerTarget == null) return;

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

    // 상태별 행동 실행
    void ExecuteStateArchitecture()
    {
        if (isDead) return;

        switch (currentState)
        {
            case AIState.Patrol:
                if (waypointNodes.Count == 0) return;

                Vector3 targetNodePos = waypointNodes[currentTargetIndex].position;
                MoveAndRotateSlerp(targetNodePos);

                if ((targetNodePos - transform.position).sqrMagnitude < 0.64f)
                {
                    currentTargetIndex = (currentTargetIndex + 1) % waypointNodes.Count;
                }
                break;

            case AIState.Chase:
                Vector3 chaseTarget = playerTarget.position;
                chaseTarget.y = transform.position.y;

                MoveAndRotateSlerp(chaseTarget);
                break;

            case AIState.Attack:
                Vector3 attackTarget = playerTarget.position;
                attackTarget.y = transform.position.y;

                MoveAndRotateSlerp(attackTarget);

                if (currentAttackCooldown <= 0f)
                {
                    ApplyBodyHitDamage();
                }
                break;
        }
    }

    // 몸박 대미지 처리
    // [수정된 부분] 몬스터 스크립트의 몸박 대미지 처리 메서드
    void ApplyBodyHitDamage()
    {
        currentAttackCooldown = attackCooldown; // 쿨타임 초기화

        if (playerTarget != null)
        {
            // 플레이어 오브젝트에서 PlayerStatus 스크립트를 찾습니다.
            PlayerStatus playerStatus = playerTarget.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                // 플레이어에게 설정해둔 데미지(bodyHitDamage)만큼 피해를 입힙니다.
                playerStatus.TakeDamage(bodyHitDamage);
                Debug.LogWarning($"[NCS 몸박 성공]: 플레이어 타겟에게 대미지 {bodyHitDamage} 부여 완료!");
            }
            else
            {
                Debug.LogError("[연동 에러]: 플레이어 타겟 오브젝트에 'PlayerStatus' 스크립트가 없습니다! 컴포넌트를 추가해주세요.");
            }
        }
    }

    // ---------------------------------------------------------------------
    // 💥 FPS 투사체 피격 시스템 (물리 충돌 이벤트)
    // ---------------------------------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // 충돌한 오브젝트의 태그가 "Bullet"(총알)인지 확인합니다.
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 예시로 총알 하나당 20의 대미지를 고정으로 준다고 가정합니다.
            // (만약 총알마다 대미지가 다르다면 총알 스크립트에서 값을 가져와야 합니다)
            float damageAmount = 20f;

            TakeDamage(damageAmount);

            // 맞은 총알 오브젝트는 부딪혔으니 맵에서 삭제합니다.
            Destroy(collision.gameObject);
        }
    }

    // 대미지를 받는 메서드 (외부 호출 가능하도록 public)
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"[몬스터 피격]: {amount} 대미지 받음. 남은 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리
    void Die()
    {
        isDead = true;
        currentState = AIState.Die;

        // 몬스터가 죽으면 더 이상 물리적으로 플레이어와 부딪히지 않게 처리
        GetComponent<Collider>().enabled = false;

        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = true; // 물리 시뮬레이션 종료
        }

        Debug.LogError("[몬스터 사망]: 몬스터가 처치되었습니다.");

        // 사망 애니메이션 연출 후 2초 뒤 오브젝트 완전 삭제
        Destroy(gameObject, 2f);
    }

    // 이동 + 회전
    void MoveAndRotateSlerp(Vector3 destination)
    {
        destination.y = transform.position.y;
        LookAtTargetSlerp(destination);

        Vector3 nextPosition = Vector3.MoveTowards(
                transform.position,
                destination,
                speed * Time.deltaTime);

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

    // Scene 뷰 기믹 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * chaseDistance);
        Gizmos.DrawRay(transform.position, rightBoundary * chaseDistance);
    }
}