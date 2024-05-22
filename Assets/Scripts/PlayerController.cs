using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float Health = 1f;
    public Transform SpawnPoint;

    [Header("Move")]
    public float MoveSpeed = 10f;
    public float Acceleration = 10f;
    public Vector2 MoveDirection;
    public Vector2 LastMoveDirection;
    [SerializeField] private bool canMove = true;

    [Header("Jump")]
    public float JumpSpeed = 20f;
    public float AirAcceleration = 7.5f;
    public float WallJumpTime = 0.2f; // time to enable movement after wall jump
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool hasJumped = false;

    [Header("Dash")]
    public float DashSpeed = 20f;
    public float DashTime = 0.2f;
    public float DashCooldownTime = 0.15f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool hasDashed = false;

    [Header("Wall")]
    public float WallSide;
    public float WallClimbSpeed = 5f;
    public float WallSlideSpeed = 5f;
    public float WallJumpUpTime = 0.25f;
    [SerializeField] private bool canClimb = true;

    [Header("Collision")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;
    // collision with ground and walls 
    // ground collision
    [SerializeField]
    private Vector2 bottomOffsetUpLeft = new Vector2(-0.4f, -0.45f),
                    // bottomOffsetUpRight = new Vector2(0.4f, -0.45f),
                    // bottomOffsetDownLeft = new Vector2(-0.4f, -0.55f),
                    bottomOffsetDownRight = new Vector2(0.4f, -0.55f);
    // right wall collision
    [SerializeField]
    private Vector2 rightOffsetUpLeft = new Vector2(0.49f, 0.4f),
                    // rightOffsetUpRight = new Vector2(0.51f, 0.4f),
                    // rightOffsetDownLeft = new Vector2(0.49f, -0.5f),
                    rightOffsetDownRight = new Vector2(0.51f, -0.4f);
    // left wall collision
    [SerializeField]
    private Vector2 leftOffsetUpLeft = new Vector2(-0.51f, 0.4f),
                    // leftOffsetUpRight = new Vector2(-0.49f, 0.4f),
                    // leftOffsetDownLeft = new Vector2(-0.51f, -0.5f),
                    leftOffsetDownRight = new Vector2(-0.49f, -0.4f);

    [Header("Particle Systems")]
    public Transform ParticleRoot;
    public ParticleSystem JumpParticle;
    public ParticleSystem WallJumpParticle;
    public ParticleSystem DashParticle;
    public ParticleSystem SlideParticle;

    [Header("Guns")]
    [SerializeField] public Transform GunPosition;
    [SerializeField] public GameObject GunObject;
    [SerializeField] private GunController _gun;
    [SerializeField] private bool _hasGun;

    [Header("Components")]
    [SerializeField] private InputController _input;
    [SerializeField] private GhostEffect _ghostEffect;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _input = GetComponent<InputController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _ghostEffect = GetComponent<GhostEffect>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _hasGun = GunObject.TryGetComponent<GunController>(out _gun);

        _ghostEffect.enabled = false;

        DashParticle.Stop();
        SlideParticle.Stop();
    }

    private void Update()
    {
        HandleCollision();
        WallClimb();
        HandleJump();
        HandleDash();
        HandleGun();
        Move();

        HandleFlipX();
    }

    private void HandleCollision()
    {
        Vector2 currentPosition = (Vector2)transform.position;
        // ground detection
        onGround = Physics2D.OverlapArea(currentPosition + bottomOffsetUpLeft, (Vector2)transform.position + bottomOffsetDownRight, groundLayer);

        // wall detection
        onRightWall = Physics2D.OverlapArea(currentPosition + rightOffsetUpLeft, (Vector2)transform.position + rightOffsetDownRight, groundLayer);
        onLeftWall = Physics2D.OverlapArea(currentPosition + leftOffsetUpLeft, (Vector2)transform.position + leftOffsetDownRight, groundLayer);

        // if player is on wall
        onWall = onRightWall || onLeftWall;

        // which side player is facing
        WallSide = onRightWall ? 1f : -1f;

        // reset jump ability
        canJump = onGround || onWall;
        hasJumped = !canJump;

        // reset dash ability
        // if player has dashed and on the ground then enable to dash again
        if (hasDashed && onGround && canDash)
        {
            hasDashed = false;
        }
    }

    private void WallClimb()
    {
        if (!canClimb)
            return;

        // check if player slides on wall
        if (onWall && !onGround)
        {
            // climb up wall
            if (_input.move.y > 0)
            {
                _rigidbody.velocity = new Vector2(0.0f, WallClimbSpeed);
            }
            // slide down wall
            else if (_input.move.y < 0)
            {
                _rigidbody.velocity = new Vector2(0.0f, -WallClimbSpeed);
            }

            // start play slide particle when player slides down wall
            if (_rigidbody.velocity.y < 0 && !SlideParticle.isPlaying)
            {
                SlideParticle.Play();
            }
            // stop slide particle when player climbs up
            else if (_rigidbody.velocity.y > 0 && SlideParticle.isPlaying)
            {
                SlideParticle.Stop();
            }
        }
        // stop slide particle when player does not slide on wall
        else
        {
            SlideParticle.Stop();
        }
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
                StartCoroutine(DisableMove(WallJumpTime));

                // wall jump up
                if (_input.move.y > 0)
                {
                    StartCoroutine(DisableClimb(WallJumpUpTime));
                    Jump(Vector2.up, true);
                }
                // wall jump
                else
                {
                    Vector2 wallJumpDir = new Vector2(-WallSide, 1f).normalized;
                    Jump(wallJumpDir, true);
                }
            }
        }
    }

    private void Jump(Vector2 jumpDir, bool isWallJump)
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.velocity += jumpDir * JumpSpeed;
        hasJumped = true;

        ParticleSystem jumpParticle = isWallJump ? WallJumpParticle : JumpParticle;
        jumpParticle.Play();
    }

    private void HandleDash()
    {
        if (!canDash || hasDashed)
            return;

        if (_input.dash)
        {
            // recalculate dash dir if player does not input move dir
            Vector2 dashDir = MoveDirection;
            // if player not inputs, dash towards face direction 
            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(_spriteRenderer.flipX ? 1f : -1f, 0f);
            }

            // disable movement and start dash
            StartCoroutine(DisableMove(DashTime));
            StartCoroutine(DisableClimb(DashTime));
            StartCoroutine(Dash(dashDir));
        }
    }

    private IEnumerator Dash(Vector2 dashDir)
    {
        // temp
        float gravityScale = _rigidbody.gravityScale;

        // start play ghost effect in DashTime
        _ghostEffect.Play(DashTime);

        hasDashed = true;
        canDash = false;

        // play dash particle
        DashParticle.Play();
        // disable gravity and move player
        _rigidbody.gravityScale = 0f;
        _rigidbody.velocity = dashDir.normalized * DashSpeed;

        // cooldown
        yield return new WaitForSeconds(DashTime);

        // stop dash particle
        DashParticle.Stop();
        // disable gravity and stop player
        _rigidbody.gravityScale = gravityScale;
        _rigidbody.velocity = Vector2.zero;

        // cooldown after dash
        yield return new WaitForSeconds(DashCooldownTime);
        canDash = true;
    }

    private void HandleGun()
    {
        if (_input.fire)
        {
            _gun.Fire();
        }
        // else if(_input.reload)
        // {

        // }
    }

    private void Move()
    {
        if (!canMove)
            return;

        MoveDirection = _input.move;

        // store the last move direction if player does move
        if (_input.move.x != 0 && _input.move.y != 0)
        {
            LastMoveDirection = MoveDirection;
        }

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

    private void HandleFlipX()
    {
        // handle player animation facing direction
        if (_input.move.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_input.move.x < 0)
        {
            _spriteRenderer.flipX = false;
        }
        // rotate gun system
        if (onWall)
        {
            GunPosition.localScale = new Vector3(WallSide * -1f, 1f, 1f);
        }
        else
        {
            GunPosition.localScale = new Vector3(_spriteRenderer.flipX ? 1f : -1f, 1f, 1f);
        }

        // rotate particle system
        ParticleRoot.localScale = new Vector3(WallSide, 1f, 1f);
    }

    private IEnumerator DisableMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private IEnumerator DisableClimb(float time)
    {
        canClimb = false;
        yield return new WaitForSeconds(time);
        canClimb = true;
    }

    public void TakeDamage()
    {
        transform.position = SpawnPoint.position;
    }
}