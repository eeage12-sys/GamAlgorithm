using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private bool isGrounded = true;
    private Camera mainCamera;

   
    private Vector3 rayStartOffset;
    private float strictCheckDistance = 0.3f; 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        if (rb != null)
        {
            
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

       
        CalculateRaycastOffset();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        
        CheckGroundedWithRaycast();

        
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            if (rb != null)
            {
                
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }

    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector3 inputDirection = Vector3.zero;

        if (keyboard.wKey.isPressed) inputDirection += Vector3.forward;
        if (keyboard.sKey.isPressed) inputDirection += Vector3.back;
        if (keyboard.aKey.isPressed) inputDirection += Vector3.left;
        if (keyboard.dKey.isPressed) inputDirection += Vector3.right;

        inputDirection = inputDirection.normalized;

        if (inputDirection != Vector3.zero && mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward = camForward.normalized;
            camRight = camRight.normalized;

            Vector3 moveDirection = (camForward * inputDirection.z + camRight * inputDirection.x).normalized;

            if (rb != null)
            {
                
                rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

                
                rb.angularVelocity = Vector3.zero;
            }

            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    
    void CalculateRaycastOffset()
    {
        if (playerCollider != null)
        {
            
            float yOffset = -(playerCollider.height / 2f) + playerCollider.center.y + 0.1f;
            rayStartOffset = new Vector3(0f, yOffset, 0f);
        }
        else
        {
            
            rayStartOffset = new Vector3(0f, -0.9f, 0f);
        }
    }

   
    void CheckGroundedWithRaycast()
    {
        
        Vector3 rayStartPos = transform.position + rayStartOffset;

        Ray ray = new Ray(rayStartPos, Vector3.down);
        RaycastHit hit;

        
        Debug.DrawRay(rayStartPos, Vector3.down * strictCheckDistance, Color.red);

       
        if (Physics.Raycast(ray, out hit, strictCheckDistance))
        {
            
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                return;
            }
        }

        isGrounded = false;
    }

    void OnDrawGizmosSelected()
    {
        CalculateRaycastOffset();
        Gizmos.color = Color.red;
        Vector3 previewPos = transform.position + rayStartOffset;
        Gizmos.DrawLine(previewPos, previewPos + Vector3.down * strictCheckDistance);
    }
}