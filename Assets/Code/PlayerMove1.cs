using UnityEngine;
using UnityEngine.InputSystem; 
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        
        Vector2 inputVector = Vector2.zero;

        
        if (Keyboard.current is not null)
        {
            float h = 0;
            float v = 0;

            
            if (Keyboard.current.aKey.isPressed) h = -1;
            if (Keyboard.current.dKey.isPressed) h = 1;
            if (Keyboard.current.wKey.isPressed) v = 1;
            if (Keyboard.current.sKey.isPressed) v = -1;

            inputVector = new Vector2(h, v);
        }

       
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        
        if (moveDir.magnitude > 0)
        {
           
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}