using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform[] controlPoints = new Transform[3]; // The control points for the Bezier curve
    private float t = 0f; // The parameter that goes from 0 to 1
    public float initialSpeed = 0.2f; // Initial speed value for smoother movement
    private float currentSpeed; // Current speed value

    private Rigidbody2D rb;
    public PhysicsMaterial2D noBounceMaterial;
    private PhysicsMaterial2D originalMaterial;
    private bool isBouncing = false;
    private bool isGrounded = false; // Flag to check if the ball is grounded

    public PlayerController player1;
    public PlayerController player2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = GetComponent<Collider2D>().sharedMaterial;
        currentSpeed = initialSpeed; // Set the initial speed
        rb.gravityScale = 1;

        if (controlPoints.Length == 3)
        {
            controlPoints[0].position = new Vector3(0, 0, 0);
            controlPoints[1].position = new Vector3(2, 5, 0);
            controlPoints[2].position = new Vector3(4, 0, 0);
        }
    }

    void Update()
    {
        if (isBouncing && !isGrounded)
        {
            // Calculate velocity and update t based on the velocity
            float velocity = currentSpeed * Time.deltaTime;
            t += velocity;
            if (t > 1f)
            {
                t = 0f; // Reset t to loop the movement, adjust as needed
            }

            // Calculate the position on the Bezier curve
            Vector3 position = CalculateBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
            transform.position = position; // Use transform.position for smoother control

            Debug.Log($"t: {t}, Velocity: {velocity}, Position: {position}"); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Stop bouncing by changing the material to a no bounce material
            GetComponent<Collider2D>().sharedMaterial = noBounceMaterial;
            isBouncing = false; // Stop the Bezier movement
            isGrounded = true; // Set isGrounded to true when grounded
            rb.velocity = Vector2.zero; // Stop any residual velocity

            // Increase score for the other player
            if (collision.transform.position.x < 0)
            {
                player2.IncreaseScore();
                transform.position = player2.transform.position + Vector3.up; // Position the ball above player 2
            }
            else
            {
                player1.IncreaseScore();
                transform.position = player1.transform.position + Vector3.up; // Position the ball above player 1
            }
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            // Ensure the original bouncy material is applied
            GetComponent<Collider2D>().sharedMaterial = originalMaterial;
            isBouncing = true; // Start the Bezier movement
            isGrounded = false; // Reset isGrounded to false when colliding with player or wall
            t = 0f; // Reset t when starting new bounce
            rb.gravityScale = 1; // Turn gravity off when following the Bezier curve

            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().InteractWithBall();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Ensure gravity is on when the ball is no longer touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.gravityScale = 1;
        }
    }



    // Function to calculate a point on a Bezier curve (Quadratic Bezier Curve)
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // First term
        p += 2 * u * t * p1; // Second term
        p += tt * p2; // Third term

        Debug.Log($"Bezier Point: {p}");

        return p;
    }
}
