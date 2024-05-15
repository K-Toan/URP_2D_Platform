using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public float Damp = 1.2f;

    [Header("Move")]
    public float MoveSpeed = 10f;
    public float Acceleration = 10f;
    public float AirAcceleration = 5f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;
    [SerializeField] private bool canMove = true;

    [Header("Jump & Gravity")]
    public float JumpSpeed = 25f;
    [SerializeField] private bool hasJumped = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool useGravity = true;

    [Header("Dash")]
    public float DashSpeed = 25f;
    public float DashTime = 0.2f;
    [SerializeField] private bool canDash = true;

    [Header("Collision")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;
    // collision with ground and walls 
    [SerializeField] private float collisionRadius = 0.01f;
    [SerializeField]
    private Vector2 bottomOffset = new Vector2(0f, -0.5f),
                                     rightOffset = new Vector2(0.5f, 0f),
                                     leftOffset = new Vector2(-0.5f, 0f);
    [SerializeField] private LayerMask groundLayer;

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
        HandleJump();
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

        // reset jump ability
        canJump = onGround || onWall;
        hasJumped = !canJump;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            // ground jump
            if (onGround)
            {
                Jump(Vector2.up, false);
            }

            // wall jump
            if (!onGround && onWall)
            {
                StartCoroutine(DisableMovement(0.25f));

                Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
                Vector2 wallJumpDir = wallDir / 1.5f + Vector2.up / 1.5f;
                Jump(wallJumpDir, true);
            }
        }
    }

    private void Jump(Vector2 jumpDir, bool wall)
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.velocity += jumpDir * JumpSpeed;
        hasJumped = true;
    }

    private void HandleDash()
    {

    }

    private IEnumerator Dash(Vector2 dashDir)
    {
        float gravityScale = _rigidbody.gravityScale;

        _rigidbody.gravityScale = 0f;
        _rigidbody.velocity = dashDir.normalized * DashSpeed;

        yield return new WaitForSeconds(0.3f);

        _rigidbody.gravityScale = _rigidbody.gravityScale;
        _rigidbody.velocity = dashDir * DashSpeed;

    }

    private void Move()
    {
        if (!canMove)
            return;

        MoveDirection = _input.move;

        // change player acceleration if player has jumped
        if (!hasJumped)
        {
            // _rigidbody.velocity = new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, (new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y)), Acceleration * Time.deltaTime);
        }
        else
        {
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, (new Vector2(MoveDirection.x * MoveSpeed, _rigidbody.velocity.y)), AirAcceleration * Time.deltaTime);
        }
    }

    private IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private IEnumerator DisableGravity(float time)
    {
        useGravity = false;
        yield return new WaitForSeconds(time);
        useGravity = true;
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