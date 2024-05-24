using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Camera MainCamera;
    private ParallaxCamera _parallaxCamera;
    [SerializeField] private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    private void Start()
    {
        MainCamera = Camera.main;

        // get parallax camera script
        if (_parallaxCamera == null)
            _parallaxCamera = MainCamera.GetComponent<ParallaxCamera>();

        if (_parallaxCamera == null)
        {
            Debug.Log("Cannot get _parallaxCamera component");
        }
        else
        {
            _parallaxCamera.OnCameraTranslate += Move;
        }

        SetLayers();
    }

    private void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Background Layer " + i;
                parallaxLayers.Add(layer);
            }
        }
    }

    private void Move(Vector2 delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);

            // calculate distance between camera position and true background posistion
            float distanceX = Mathf.Abs(MainCamera.transform.position.x - layer.transform.position.x);

            // reposition background
            if(distanceX >= layer.TextureWidthX)
            {
                float offsetPosition = distanceX % layer.TextureWidthX;
                layer.transform.position = new Vector3(MainCamera.transform.position.x + offsetPosition, layer.transform.position.y);
            }
        }
    }
}
