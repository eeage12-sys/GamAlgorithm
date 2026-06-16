using UnityEngine;
using UnityEngine.InputSystem;

public class QuaternionInputGizmoPractice : MonoBehaviour
{
    public float rotationSpeed = 4f;
    public float targetMoveSpeed = 3f;
    public float targetDistance = 4f;
    public float targetRange = 3f;

    Vector3 targetOffset = new Vector3(0f, 0f, 4f);

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        Vector3 input = Vector3.zero;

        
        if (keyboard.leftArrowKey.isPressed)
        {
            input.x -= 1f;
        }

        if ( keyboard.rightArrowKey.isPressed)
        {
            input.x += 1f;
        }

        if (keyboard.downArrowKey.isPressed)
        {
            input.z -= 1f;
        }

        if (keyboard.upArrowKey.isPressed)
        {
            input.z += 1f;
        }

        if (keyboard.shiftKey.isPressed)
        {
            input.y -= 1f;
        }

        if (keyboard.ctrlKey.isPressed)
        {
            input.y += 1f;
        }

        
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            targetOffset = new Vector3(0f, 0f, 0f);
        }

        targetOffset += new Vector3(input.x, input.y, input.z) * targetMoveSpeed * Time.deltaTime;
        targetOffset.x = Mathf.Clamp(targetOffset.x, -targetRange, targetRange);
        targetOffset.y = Mathf.Clamp(targetOffset.y, -targetRange, targetRange);
        targetOffset.z = Mathf.Clamp(targetOffset.z, -targetRange, targetRange);

        Vector3 targetDirection = targetOffset.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 targetPosition = origin + targetOffset;
        Vector3 targetDirection = targetOffset.normalized;

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPosition, 0.15f);

        Gizmos.color = Color.blue;
        
        Gizmos.DrawLine(origin, origin + transform.forward * targetDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + targetDirection * targetDistance);
    }
}