using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 10f;
    public float Acceleration = 15f;
    [SerializeField] private Vector2 MoveDirection;

    [Header("Jump & Gravity")]
    public float JumpForce = 15f;
    public float FallMultiplier = 5f;
    public float LowJumpMultiplier = 7.5f;

    [Header("Collision")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;
    [SerializeField] private bool wallGrab;
    [SerializeField] private int wallSide;
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 rightOffset;
    [SerializeField] private Vector2 leftOffset;
    public LayerMask groundLayer;
    private Color _debugCollisionColor = Color.green;

    [Header("Booleans")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool canJump = true;

    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        bottomOffset = new Vector2(0f, -0.5f);
        leftOffset = new Vector2(-0.5f, 0f);
        rightOffset = new Vector2(0.5f, 0f);
    }

    private void Update()
    {
        HandleCollision();
        HandleWallSlide();
        HandleGravity();
        Jump();
        Move();

        FlipSprite();
    }

    private void HandleCollision()
    {
        // check if player is on the ground
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        // check if player is on left or right wall
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);

        // if player is colliding with wall
        onWall = onRightWall || onLeftWall;

        wallSide = onRightWall ? -1 : 1;
    }

    private void HandleWallSlide()
    {
        wallGrab = Input.GetKey(KeyCode.W) && onWall;

        if (onWall && !onGround)
        {
            if (wallGrab)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, MoveSpeed);
            }
            else
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -3f);
            }
        }
    }

    private void HandleGravity()
    {
        // Handle gravity and jump height
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * FallMultiplier * Time.deltaTime;
        }
        else if (_rigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * LowJumpMultiplier * Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                // Ground jump
                if (onGround && !onWall)
                {
                    Debug.Log("Ground Jump");
                    _rigidbody.velocity += Vector2.up * JumpForce;
                }
                // Wall jump
                else if (onWall)
                {
                    Debug.Log("Wall Jump");

                    Vector2 jumpDirection = new Vector2(onRightWall ? -1f : 1f, 1f).normalized;
                    _rigidbody.velocity += jumpDirection * JumpForce;

                    StartCoroutine(DisableMovement(0.25f));
                }
            }
        }
    }

    private void Dash()
    {

    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        MoveDirection = new Vector2(x, y);

        if (canMove)
        {
            // _rigidbody.velocity = new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, (new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y)), Acceleration * Time.deltaTime);
        }
    }

    public IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public IEnumerator JumpCooldown(float time)
    {
        canJump = false;
        yield return new WaitForSeconds(time);
        canJump = true;
    }

    public IEnumerator DashCooldown(float time)
    {
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }

    void FlipSprite()
    {
        if (MoveDirection.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (MoveDirection.x < 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
}
