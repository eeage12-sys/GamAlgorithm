using UnityEngine;

public class PhysicsEventViewer : MonoBehaviour
{
    public string currentState = "대기 중";
    private Color gizmoColor = Color.gray;

    private void OnCollisionEnter(Collision collision)
    {
        // 실제로 부딪히는 충돌이 시작될 때 호출됩니다.
        currentState = "Collision Enter: " + collision.gameObject.name;
        gizmoColor = Color.red;
    }

    private void OnCollisionExit(Collision collision)
    {
        // 실제 물리 충돌이 끝나서 떨어졌을 때 호출됩니다.
        currentState = "Collision Exit: " + collision.gameObject.name;
        gizmoColor = Color.yellow;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 통과 가능한 감지 영역에 들어왔을 때 호출됩니다.
        currentState = "Trigger Enter: " + other.gameObject.name;
        gizmoColor = Color.cyan;
    }

    private void OnTriggerExit(Collider other)
    {
        // 통과 가능한 감지 영역에서 나갔을 때 호출됩니다.
        currentState = "Trigger Exit: " + other.gameObject.name;
        gizmoColor = Color.blue;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}