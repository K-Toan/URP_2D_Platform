using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float MoveSpeed = 5f;
    public Vector2 MoveDir;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rigidbody.velocity = MoveDir.normalized * MoveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (MoveDir.x * player.MoveDirection.x >= 0)
            {
                Debug.Log("cùng hướng");
                player.TakeDamage();
            }
            else
            {
                Debug.Log("khác hướng");
                Destroy(gameObject);
            }
            MoveDir.x *= -1f;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            // enemy bounce dir when collide with wall
            Vector2 normal = other.contacts[0].normal;
            Vector2 bounceDirection = Vector2.Reflect(MoveDir.normalized, normal);

            MoveDir = bounceDirection;
        }
    }
}
