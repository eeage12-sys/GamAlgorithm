using UnityEngine;

public class SimpleCollision : MonoBehaviour
{
    public Transform other;
    public float radiusA = 1.0f;
    public float radiusB = 1.0f;

    private bool IsOverlapping()
    {
        if (other == null)
        {
            return false;
        }

        Vector3 diff = transform.position - other.position;

        
        float distanceSq = diff.sqrMagnitude;

        float radiusSum = radiusA + radiusB;
        float radiusSumSq = radiusSum * radiusSum;

        return distanceSq <= radiusSumSq;
    }

    private void OnDrawGizmos()
    {
        if (other == null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, radiusA);
            return;
        }

        bool isOverlapping = IsOverlapping();

        Gizmos.color = isOverlapping ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusA);
        Gizmos.DrawWireSphere(other.position, radiusB);

        Gizmos.DrawLine(transform.position, other.position);
    }
}