using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    // 기즈모를 사용하여 에디터 뷰에서 실시간으로 확인
    // OnDrawGizmos는 씬 뷰에 디버그용 도형을 그릴 때 유니티가 자동 호출하는 메서드입니다.
    public void OnDrawGizmos()
    {
        // 1. 현재 오브젝트의 로컬->월드 변환 행렬 가져오기
        // (내부적으로 Translation * Rotation * Scale이 결합된 상태)
        // transform.localToWorldMatrix는 로컬 좌표를 월드 좌표로 바꾸는 변환 행렬 프로퍼티입니다.
        Matrix4x4 worldMatrix = transform.localToWorldMatrix;

        // 2. 로컬 상의 특정 점 (예: 내 오른쪽으로 2미터 지점)
        Vector3 localPos = new Vector3(2f, 0, 0);

        // 3. 행렬 곱을 통해 월드 좌표로 변환
        // MultiplyPoint3x4는 4x4 행렬을 3D 점에 적용하는 가장 일반적인 방법입니다.
        Vector3 worldPos = worldMatrix.MultiplyPoint3x4(localPos);

        // 시각화
        // Gizmos.color는 이후에 그릴 기즈모 도형의 색상을 지정하는 프로퍼티입니다.
        // Color.yellow는 유니티가 미리 제공하는 노란색 값입니다.
        Gizmos.color = Color.yellow;
        // Gizmos.DrawLine은 씬 뷰에 두 점을 잇는 선을 그리는 메서드입니다.
        Gizmos.DrawLine(transform.position, worldPos); // 원점에서 변환된 점까지 선 그리기
        // Gizmos.DrawSphere는 씬 뷰에 작은 구를 그려 특정 위치를 표시하는 메서드입니다.
        Gizmos.DrawSphere(worldPos, 0.1f);            // 변환된 최종 위치에 구체 그리기

        // 결과 출력 (프레임마다 찍히지 않도록 필요 시에만 사용)
        // Debug.Log($"로컬(2,0,0) -> 월드({worldPos})");
    }
}