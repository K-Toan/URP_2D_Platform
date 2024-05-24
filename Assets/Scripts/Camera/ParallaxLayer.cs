using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Vector2 ParallaxFactor;

    public float TextureWidthX;
    public float TextureWidthY;

    [Header("Components")]
    [SerializeField] private Sprite sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;

        TextureWidthX = sprite.texture.width / sprite.pixelsPerUnit;
        TextureWidthY = sprite.texture.height / sprite.pixelsPerUnit;
    }

    public void Move(Vector3 delta)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition -= new Vector3(delta.x * ParallaxFactor.x, delta.y * ParallaxFactor.y);

        transform.localPosition = newPosition;
    }
}