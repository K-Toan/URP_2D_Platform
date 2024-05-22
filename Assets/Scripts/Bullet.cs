using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public float Speed = 10f;
    public float Damage = 1f;
    public Vector2 Direction;

    [Header("Components")]
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rigidbody.velocity = Direction * Speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Environment")
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Enemy")
        {

        }
    }
}
