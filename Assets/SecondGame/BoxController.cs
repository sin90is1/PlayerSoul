using UnityEngine;

public class BoxController : MonoBehaviour
{
    public float groundMoveForce = 10f; // Force applied on normal ground
    public float maxSpeed = 8f; // Maximum speed the ball can reach on ground
    public float groundDrag = 3f; // Drag on normal ground (higher for less sliding)
    public float jumpForce = 10f; // Force applied for the jump


    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private bool isGrounded = false; // To check if the ball is on the ground


    void Start()
    {
        // Get the Rigidbody2D component attached to the ball
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from keyboard (WASD or arrow keys)
        float moveX = Input.GetAxis("Horizontal"); // Left (-1) and Right (+1)

        // Apply force to the ball based on input and surface type
        float currentMaxSpeed = maxSpeed;
        if (Mathf.Abs(rb.velocity.x) < currentMaxSpeed)
        {
            float moveForce = groundMoveForce;
            rb.AddForce(new Vector2(moveX * moveForce, 0f));
        }

        // Apply drag only to the X-axis
        float currentDrag = groundDrag;
        rb.velocity = new Vector2(rb.velocity.x * (1 - Time.deltaTime * currentDrag), rb.velocity.y);

        // Check for jump input and if the ball is grounded or on a wall
        if (Input.GetButtonDown("Jump") && (isGrounded))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // Temporarily disable jumping until we land
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the ball is colliding with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the ball is no longer colliding with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Ball is in the air
        }
 
    }
}