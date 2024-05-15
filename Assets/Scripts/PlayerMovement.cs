using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public float Damp = 1.2f;

    [Header("Move")]
    public float MoveSpeed = 6f;
    public float Acceleration = 6f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;

    [Header("Jump")]
    public float JumpSpeed = 25f;

    [Header("Collision")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;
    // collision with ground and walls 
    [SerializeField] private float collisionRadius = 0.1f;
    [SerializeField] private Vector2 bottomOffset = new Vector2(0f, -0.5f),
                                     rightOffset = new Vector2(0.5f, 0f),
                                     leftOffset = new Vector2(-0.5f, 0f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash")]
    public float DashSpeed = 10f;
    public float DashTime = 0.2f;

    [Header("Components")]
    [SerializeField] private InputController _input;
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start()
    {
        _input = GetComponent<InputController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleCollision();
        Jump();
        Move();
    }

    private void HandleCollision()
    {
        // ground detection
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        // wall detection
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        onWall = onRightWall || onLeftWall;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ground jump
            if (onGround)
            {
                _rigidbody.velocity += Vector2.up * JumpSpeed;
            }

            // wall jump
            if (!onGround && onWall)
            {
                Vector2 wallJumpDir = new Vector2(onRightWall ? -1.0f : 1.0f, 1.0f).normalized * JumpSpeed;
                StartCoroutine(WallJump(wallJumpDir));
            }
        }
    }

    private void Move()
    {
        if (_input.move.x != 0 && _input.move.y != 0)
        {
            LastMoveDirection = _input.move;
        }
        MoveDirection = _input.move;

        // move with acceleration/force-like
        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, (new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y)), Acceleration * Time.deltaTime);
    }

    private IEnumerator WallJump(Vector2 jumpDir)
    {
        _rigidbody.velocity = jumpDir;
        yield return new WaitForSeconds(0.5f);
    }
}