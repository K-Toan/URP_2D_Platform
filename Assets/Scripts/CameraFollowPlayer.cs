using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Stats")]
    public float Damp = 1.2f;

    [Header("Followed target")]
    public Transform Target;

    private void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(Target.position.x, Target.position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Damp * Time.deltaTime);
    }
}