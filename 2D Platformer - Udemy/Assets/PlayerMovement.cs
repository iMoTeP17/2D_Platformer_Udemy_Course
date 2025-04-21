using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int Grounded = Animator.StringToHash("isGrounded");
    private static readonly int IsWallSliding = Animator.StringToHash("isWallSliding");
    private static readonly int IsWallDectected = Animator.StringToHash("isWallDectected");

    // NOTE: public variables, if its private it doesn't get picked up in Unity editor
    
    private  Rigidbody2D _rigidBody;
    private Animator _animator;
    
    [Header("Movement Info")]
    public int moveSpeed;
    private float _movementInputX;
    private const float JumpForce = 16f;
    //private bool _canDoubleJump;
    private bool _canMove;
    
    
    [Header("Ground Collision Info")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    private const float GroundCheckRadius = 0.2f;


    // Wall Detection
    private bool _isWallDetected;
    public  float wallCheckDistance;
    
    // Wall slide
    private bool _isWallSliding;
    
    
    
    private bool _isFacingRight = true;
    private int _facingDirection = 1;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationController();
        FlipController();
        WallCollisionCheck();
        CanWallSlide();
        CalculateMovementX();

        if (IsGrounded())
        {
            _canMove = true;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _movementInputX = context.ReadValue<Vector2>().x;
    }
    
    private void CalculateMovementX()
    {
        if (_canMove)
            _rigidBody.linearVelocity = new Vector2(_movementInputX * moveSpeed, _rigidBody.linearVelocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, JumpForce);
            //_canDoubleJump = true;
        }
        
        // Longer press jumps higher
        if (context.canceled && _rigidBody.linearVelocity.y > 0f)
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _rigidBody.linearVelocity.y * 0.5f);
        }
        
        // TODO add Double Jump, causes issues with wall jump
        /*if (context.performed && !IsGrounded() && _canDoubleJump && !_isWallSliding)
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, JumpForce);
            _canDoubleJump = false;
            Debug.Log("I am DOUBLE JUMPING");
        }*/
        
        // Wall Jump
        if (context.performed && CanWallJump())
        {
            _canMove = false;
            _isWallSliding = false;
            _rigidBody.linearVelocity = new Vector2(7 * -_facingDirection, JumpForce+1f);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayer);
    }

    private void CanWallSlide()
    {
        if (_isWallDetected && IsCharacterFalling())
        {
            _isWallSliding = true;
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocityY * 0.01f);
        }
        else if (!_isWallDetected)
        {
            _isWallSliding = false;
        }
    }

    private bool CanWallJump()
    {
        return _isWallSliding;
    }
    

    private void WallCollisionCheck()
    {       
        _isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * _facingDirection, wallCheckDistance, groundLayer);
    }

    private bool IsCharacterFalling()
    {
        return _rigidBody.linearVelocity.y < 0f;
    }
    

    private void AnimationController()
    {
        // set we are moving left or right
        bool isMoving = _rigidBody.linearVelocityX != 0f;
        _animator.SetBool(IsMoving, isMoving);

        // set we are moving in air
        _animator.SetFloat(YVelocity, _rigidBody.linearVelocity.y);
        
        // set we are touching a platform
        _animator.SetBool(Grounded, IsGrounded());
        
        // set we are sliding against a wall
        _animator.SetBool(IsWallDectected, _isWallDetected);
        _animator.SetBool(IsWallSliding, _isWallSliding);

        
    }

    // Control Direction Character is facing
    private void FlipController()
    {
        if (_isFacingRight && _rigidBody.linearVelocityX < 0f)
        {
            Flip();
        }
        else if (!_isFacingRight && _rigidBody.linearVelocityX > 0f)
        {
            Flip();
        }
    }
    
    private void Flip()
    {
        _facingDirection *= -1;
        _isFacingRight = !_isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckDistance * _facingDirection, transform.position.y));
    }
}
