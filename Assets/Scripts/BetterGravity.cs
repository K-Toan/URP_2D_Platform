using UnityEngine;

public class BetterGravity : MonoBehaviour
{
    [SerializeField] float FallMultiplier = 4f;

    private Vector2 gravityDirection;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        gravityDirection = new Vector2(0, -Physics2D.gravity.y);

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += new Vector2(0, Physics2D.gravity.y) * FallMultiplier * Time.deltaTime;
        }
    }
}