using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("벽과 충돌!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("센서 진입!");
    }
}