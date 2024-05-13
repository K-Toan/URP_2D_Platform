using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 10f;
    public float ClimbSpeed = 3f;
    public float Acceleration = 10f;
    [SerializeField] private Vector2 MoveDirection;
    [SerializeField] private Vector2 LastMoveDirection;

    [Header("Jump & Gravity")]
    public float JumpSpeed = 15f;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;
    public float JumpCooldownTime = 0.2f;
    private bool enhanceGravity = true;

    [Header("Dash")]
    public float DashSpeed = 30f;
    public float DashTime = 0.3f;
    public float DashCooldownTime = 0.5f;

    [Header("Collision")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;
    [SerializeField] private bool wallClimb;
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
    // [SerializeField] private bool wallJumped = false;

    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private InputController _input;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _input = GetComponent<InputController>();

        bottomOffset = new Vector2(0f, -0.5f);
        leftOffset = new Vector2(-0.5f, 0f);
        rightOffset = new Vector2(0.5f, 0f);
    }

    private void Update()
    {
        HandleCollision();
        HandleWallSlide();
        HandleGravity();
        HandleJump();
        Dash();
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
    }

    private void HandleWallSlide()
    {
        // detect if player is on wall and climbing up
        wallClimb = onWall && _input.move.y > 0;

        if (onWall && !onGround)
        {
            if (wallClimb)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, ClimbSpeed);
            }
            else
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -Mathf.Sqrt(ClimbSpeed));
            }
        }
    }

    private void HandleGravity()
    {
        if (enhanceGravity)
        {
            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * FallMultiplier * Time.deltaTime;
            }
            else if (_rigidbody.velocity.y > 0 && !_input.jump)
            {
                _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * LowJumpMultiplier * Time.deltaTime;
            }
        }
    }

    private void HandleJump()
    {
        if (canJump && _input.jump)
        {
            if (onGround)
            {
                GroundJump();
            }
            else if (onWall)
            {
                WallJump();
            }
            StartCoroutine(JumpCooldown(JumpCooldownTime));
        }
    }

    private void GroundJump()
    {
        Debug.Log("Ground Jump");

        _rigidbody.velocity += Vector2.up * JumpSpeed;

        StartCoroutine(JumpCooldown(0.5f));
    }

    private void WallJump()
    {
        Debug.Log("Wall Jump");

        _rigidbody.velocity = new Vector2(onRightWall ? -1f : 1f, 1f).normalized * JumpSpeed;

        StartCoroutine(DisableMovement(0.25f));
    }

    private void Dash()
    {
        if (canDash && _input.dash)
        {
            // Vector2 dashDirection = new Vector2(MoveDirection.x, MoveDirection.y).normalized;
            Vector2 dashDirection = LastMoveDirection.normalized;

            // start dashing
            StartCoroutine(Dash(dashDirection));
        }
    }

    private void Move()
    {
        if (_input.move != Vector2.zero)
            LastMoveDirection = _input.move;

        MoveDirection = _input.move;

        if (!canMove || wallClimb)
            return;

        if (onGround)
        {
            _rigidbody.velocity = new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, (new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y)), Acceleration * Time.deltaTime);
        }
    }

    private void FlipSprite()
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

    public IEnumerator Dash(Vector2 dashDirection)
    {
        StartCoroutine(DashCooldown(DashCooldownTime));

        // temp
        float gravityScale = _rigidbody.gravityScale;

        // start dashing
        _rigidbody.gravityScale = 0f;
        _rigidbody.velocity = dashDirection * DashSpeed;

        yield return new WaitForSeconds(DashTime);

        // reset
        _rigidbody.gravityScale = gravityScale;
        _rigidbody.velocity = Vector2.zero;
    }

    public IEnumerator DashCooldown(float time)
    {
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
}
