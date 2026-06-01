using UnityEngine;

public class SimpleCollision : MonoBehaviour
{
    public Transform other;
    public float radiusA = 1.0f;
    public float radiusB = 1.0f;

    // 두 구체가 겹쳐있는지 판단하는 함수
    private bool IsOverlapping()
    {
        if (other == null)
        {
            return false;
        }

        // 1. 두 물체의 위치 차이를 벡터로 구합니다.
        Vector3 diff = transform.position - other.position;

        // 2. sqrMagnitude는 벡터 길이의 제곱입니다. (x^2 + y^2 + z^2)
        // 제곱근(루트) 계산 없이 거리 비교를 할 수 있어 연산이 매우 빠릅니다.
        float distanceSq = diff.sqrMagnitude;

        // 3. 두 반지름을 더한 뒤 제곱합니다.
        float radiusSum = radiusA + radiusB;
        float radiusSumSq = radiusSum * radiusSum;

        // 4. 거리의 제곱이 반지름 합의 제곱보다 작거나 같으면 두 구가 겹친 것입니다.
        return distanceSq <= radiusSumSq;
    }

    // Scene 뷰에 개발용 시각 가이드를 그리는 Unity 메시지 메서드
    private void OnDrawGizmos()
    {
        if (other == null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, radiusA);
            return;
        }

        // 충돌 여부 체크
        bool isOverlapping = IsOverlapping();

        // 5. 충돌하지 않으면 초록색, 충돌하면 빨간색으로 범위를 표시합니다.
        Gizmos.color = isOverlapping ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusA);
        Gizmos.DrawWireSphere(other.position, radiusB);

        // 두 중심점 사이의 거리를 눈으로 확인할 수 있도록 선을 그립니다.
        Gizmos.DrawLine(transform.position, other.position);
    }
}