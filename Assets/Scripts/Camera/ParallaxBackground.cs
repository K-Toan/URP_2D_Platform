using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera ParallaxCamera;
    [SerializeField] private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void Start()
    {
        if (ParallaxCamera == null)
            ParallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        if (ParallaxCamera == null)
        {
            Debug.Log("Cannot get ParallaxCamera component");
        }
        else
        {
            ParallaxCamera.onCameraTranslate += MoveVector2;
        }

        SetLayers();
    }

    void SetLayers()
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

    void MoveVector2(Vector2 delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.MoveVector2(delta);
        }
    }
}
