using UnityEngine;
 
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;
 
    private Vector3 lastPosition;
 
    void Start()
    {
        lastPosition = transform.position;
    }
 
    void Update()
    {
        if (transform.position != lastPosition)
        {
            if (OnCameraTranslate != null)
            {
                Vector3 delta = lastPosition - transform.position;
                OnCameraTranslate(delta);
            }
 
            lastPosition = transform.position;
        }
    }
}