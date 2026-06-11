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
        Die
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

    //  [새로 추가된 UI 연동 변수]
    [Header("UI 연동")]
    public MonsterHPBar hpBarScript;

    void Start()
    {
        // 게임 시작 시 체력 초기화
        currentHealth = maxHealth;

        //  [새로 추가] 시작할 때 HP 바를 100% 상태로 초기화합니다.
        if (hpBarScript != null)
        {
            hpBarScript.UpdateHPBar(currentHealth, maxHealth);
        }
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
    void ApplyBodyHitDamage()
    {
        currentAttackCooldown = attackCooldown; // 쿨타임 초기화

        if (playerTarget != null)
        {
            PlayerStatus playerStatus = playerTarget.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                playerStatus.TakeDamage(bodyHitDamage);
                Debug.LogWarning($"[NCS 몸박 성공]: 플레이어 타겟에게 대미지 {bodyHitDamage} 부여 완료!");
            }
            else
            {
                Debug.LogError("[연동 에러]: 플레이어 타겟 오브젝트에 'PlayerStatus' 스크립트가 없습니다! 컴포넌트를 추가해주세요.");
            }
        }
    }

    // FPS 투사체 피격 시스템 (물리 충돌 이벤트)
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            float damageAmount = 20f;
            TakeDamage(damageAmount);
            Destroy(collision.gameObject);
        }
    }

    // 대미지를 받는 메서드
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"[몬스터 피격]: {amount} 대미지 받음. 남은 체력: {currentHealth}/{maxHealth}");

        // [새로 추가] 피격당할 때마다 피통 UI를 실시간으로 업데이트합니다.
        if (hpBarScript != null)
        {
            hpBarScript.UpdateHPBar(currentHealth, maxHealth);
        }

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

        // [새로 추가] 몬스터가 죽으면 머리 위 피통 UI를 화면에서 숨깁니다.
        if (hpBarScript != null)
        {
            hpBarScript.gameObject.SetActive(false);
        }

        GetComponent<Collider>().enabled = false;

        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        Debug.LogError("[몬스터 사망]: 몬스터가 처치되었습니다.");

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