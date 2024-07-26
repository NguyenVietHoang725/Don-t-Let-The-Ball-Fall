using System.Collections;
using UnityEngine;
using TMPro; // Add this line to use TextMeshPro

public class BallController : MonoBehaviour
{
    public float initialSpeed = 5f; // Initial speed of the ball
    private Rigidbody2D rb;
    public PhysicsMaterial2D noBounceMaterial;
    private PhysicsMaterial2D originalMaterial;
    private bool isGrounded = false; // Flag to check if the ball is grounded

    public PlayerController player1;
    public PlayerController player2;
    public TMP_Text countdownText; // Reference to the TextMeshPro Text for countdown
    public float countdownDuration = 3f; // Countdown duration

    private PlayerController lastRoundWinner;
    public ScoreManager scoreManager; // Reference to ScoreManager

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = GetComponent<Collider2D>().sharedMaterial;

        // Ensure initial conditions are set
        rb.gravityScale = 0; // Disable gravity to prevent the ball from falling
        rb.velocity = Vector2.zero; // Ensure the ball has no initial velocity

        // Start the first round with a countdown
        StartCoroutine(StartFirstRound());
    }

    IEnumerator StartFirstRound()
    {
        // Set the ball position to a fixed starting point
        transform.position = new Vector3(0.19f, 4f, 0); // Example position; adjust as needed

        // Ensure the ball does not move during the countdown
        rb.gravityScale = 0; // Disable gravity
        rb.velocity = Vector2.zero; // Ensure no velocity

        // Display countdown before launching the ball
        for (float i = countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "Start!";
        yield return new WaitForSeconds(1f);
        countdownText.text = "";

        // Launch the ball in a random direction for the first round
        LaunchBall();
    }

    void LaunchBall()
    {
        Vector2 launchDirection = new Vector2(Random.Range(-1f, 1f), 1).normalized;
        rb.velocity = launchDirection * initialSpeed;

        // Enable gravity
        rb.gravityScale = 1;
    }

    IEnumerator StartNewRound()
    {
        // Set the ball position above the player who won the previous round
        Vector3 spawnPosition = lastRoundWinner.transform.position + Vector3.up * 7f; // Adjust offset if needed
        transform.position = spawnPosition;

        // Ensure the ball does not move during the countdown
        rb.gravityScale = 0; // Disable gravity
        rb.velocity = Vector2.zero; // Ensure no velocity

        // Display countdown before letting the ball fall
        for (float i = countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "Start!";
        yield return new WaitForSeconds(1f);
        countdownText.text = "";

        // Enable gravity and let the ball fall
        rb.gravityScale = 1;
    }

    void ResetPlayerPositions()
    {
        // Set player positions to their initial positions (if needed)
        player1.transform.position = new Vector3(-7f, player1.transform.position.y, player1.transform.position.z);
        player2.transform.position = new Vector3(7.19f, player2.transform.position.y, player2.transform.position.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                // Stop bouncing by changing the material to a no bounce material
                GetComponent<Collider2D>().sharedMaterial = noBounceMaterial;
                rb.velocity = Vector2.zero; // Stop any residual velocity
                isGrounded = true; // Set isGrounded to true when grounded

                // Determine the winner of the round
                if (transform.position.x < 0)
                {
                    scoreManager.IncreaseScore(player2);
                    lastRoundWinner = player2;
                }
                else
                {
                    scoreManager.IncreaseScore(player1);
                    lastRoundWinner = player1;
                }

                // Reset the ball material
                GetComponent<Collider2D>().sharedMaterial = originalMaterial;

                // Disable gravity and stop the ball to start a new round
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;

                // Reset player positions
                ResetPlayerPositions();

                // Start a new round with countdown
                StartCoroutine(StartNewRound());
            }
        }                
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = false; // Reset isGrounded to false when colliding with player or wall

            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().InteractWithBall();
            }
        }
    }
}
