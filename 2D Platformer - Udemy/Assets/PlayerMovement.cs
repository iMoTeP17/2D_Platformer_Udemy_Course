using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    // NOTE: public variables, if its private it doesnt get picked up in Unity editor
    
    private  Rigidbody2D rigidBody;
    private Animator animator;
    
    [Header("Movement Info")]
    public int moveSpeed;
    private float movementInputX;
    private float jumpForce = 16f;
    private bool canDoubleJump;
    
    [Header("Ground Collision Info")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        doMovement();
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }


    public void Move(InputAction.CallbackContext context)
    {
        movementInputX = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
            canDoubleJump = true;

        }
        
        // Longer press jumps higher
        if (context.canceled && rigidBody.linearVelocity.y > 0f)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, rigidBody.linearVelocity.y * 0.5f);
        }
        
        // Double Jump
        if (context.performed && !isGrounded() && canDoubleJump)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }
    }

    private void doMovement()
    {
        bool isMoving = rigidBody.linearVelocityX != 0f;
        animator.SetBool("isMoving", isMoving);
        
        rigidBody.linearVelocity = new Vector2(movementInputX* moveSpeed, rigidBody.linearVelocity.y);
    }
    
}
