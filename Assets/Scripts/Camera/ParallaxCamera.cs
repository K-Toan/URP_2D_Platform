using UnityEngine;
 
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
 
    private Vector3 lastPosition;
 
    void Start()
    {
        lastPosition = transform.position;
    }
 
    void Update()
    {
        if (transform.position != lastPosition)
        {
            if (onCameraTranslate != null)
            {
                Vector3 delta = lastPosition - transform.position;
                onCameraTranslate(delta);
            }
 
            lastPosition = transform.position;
        }
    }
}