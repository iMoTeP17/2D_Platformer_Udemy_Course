using UnityEngine;

public class Player : MonoBehaviour
{

    // NOTE: public variables, if its private it doesnt get picked up in Unity editor
    
    public int moveSpeed;
    public Rigidbody2D rigidBody;
    public int jumpForce;
    
    
    private float movementInputX;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        movementInputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown((KeyCode.Space)))
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
        }
        
        rigidBody.linearVelocity = new Vector2(moveSpeed * movementInputX, rigidBody.linearVelocity.y);
    }
}
