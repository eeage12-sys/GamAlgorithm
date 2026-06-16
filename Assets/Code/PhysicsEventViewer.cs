using UnityEngine;

public class PhysicsEventViewer : MonoBehaviour
{
    public string currentState = "´ë±â Áß";
    private Color gizmoColor = Color.gray;

    private void OnCollisionEnter(Collision collision)
    {
        currentState = "Collision Enter: " + collision.gameObject.name;
        gizmoColor = Color.red;
    }

    private void OnCollisionExit(Collision collision)
    {
        currentState = "Collision Exit: " + collision.gameObject.name;
        gizmoColor = Color.yellow;
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState = "Trigger Enter: " + other.gameObject.name;
        gizmoColor = Color.cyan;
    }

    private void OnTriggerExit(Collider other)
    {
        currentState = "Trigger Exit: " + other.gameObject.name;
        gizmoColor = Color.blue;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}