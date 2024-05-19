using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float MoveSpeed = 5f;
    public Vector2 MoveDir = Vector2.zero;

    private float moveSpeed;
    private Vector2 moveDir;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Redirect();
    }

    private void Redirect()
    {
        // moveSpeed = Random.value * MoveSpeed;
        moveSpeed = Random.value * MoveSpeed + 2f;
        moveDir = Vector2.left;
    }

    private void Update()
    {
        _rigidbody.velocity = moveSpeed * moveDir;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (moveDir.x * player.MoveDirection.x >= 0)
            {
                Debug.Log("cùng hướng");
                player.TakeDamage();
            }
            else
            {
                Debug.Log("khác hướng");
                Destroy(gameObject);
            }
            moveDir.x *= -1f;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            // Nếu va chạm với environment, đảo hướng di chuyển của enemy
            moveDir.x *= -1f;
        }
    }
}
