using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float MoveSpeed = 5f;
    public Vector2 MoveDirection;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rigidbody.velocity = MoveDirection.normalized * MoveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (MoveDirection.x * player.MoveDirection.x >= 0)
            {
                player.TakeDamage();
            }
            else
            {
                // Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
        
        // enemy bounce dir when collide with gameobject
        Vector2 normal = other.contacts[0].normal;
        Vector2 bounceDirection = Vector2.Reflect(MoveDirection.normalized, normal);

        MoveDirection = bounceDirection;
    }
}
