using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Vector2 parallaxFactor;

    public void MoveVector2(Vector3 delta)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition -= new Vector3(delta.x * parallaxFactor.x, delta.y * parallaxFactor.y);

        transform.localPosition = newPosition;
    }
}