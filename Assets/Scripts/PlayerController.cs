using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;

    [Header("Movement")]
    public float moveSpeed = 7.0f;
    private float _horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10.0f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2.0f;
    public float maxFallSpeed = 18.0f;
    public float fallSpeedMultiplier = 2.0f;

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    public Sprite originalSprite; // The original sprite
    public Sprite interactSprite; // The sprite to show when interacting with the ball
    public float spriteChangeDuration = 0.5f; // Duration for how long to show the interact sprite

    [Header("Score")]
    public int score = 0;

    void Start()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        _rb.velocity = new Vector2(_horizontalMovement * moveSpeed, _rb.velocity.y);
        GroundCheck();
        Gravity();
    }

    private void Gravity()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.gravityScale = baseGravity * fallSpeedMultiplier;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            _rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            if (context.performed)
            {
                // Hold down jump button = full height
                _rb.velocity = new Vector2(_rb.velocity.y, jumpPower);
            }
            else if (context.canceled)
            {
                // Light tap of jump button = half the height
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
            }
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    // Coroutine to change the player's sprite and revert back after a delay
    private IEnumerator ChangeSprite()
    {
        spriteRenderer.sprite = interactSprite;
        yield return new WaitForSeconds(spriteChangeDuration);
        spriteRenderer.sprite = originalSprite;
    }

    // Method to trigger the sprite change coroutine
    public void InteractWithBall()
    {
        StartCoroutine(ChangeSprite());
    }

    // Method to increase the score
    public void IncreaseScore()
    {
        score++;
        Debug.Log($"{gameObject.name} Score: {score}");
        // Optionally, update the UI here
    }
}
