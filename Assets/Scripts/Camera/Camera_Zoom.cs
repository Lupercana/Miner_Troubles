using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoom : MonoBehaviour
{
    [SerializeField] private Camera ref_camera = null;
    [SerializeField] private float zoom_ortho_min = 0f;
    [SerializeField] private float zoom_ortho_max = 0f;
    [SerializeField] private float zoom_speed = 0f;

    private float zoom_current;

    private void Start()
    {
        ref_camera.orthographic = true;
        zoom_current = ref_camera.orthographicSize;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * -1;
        if (scroll != 0f)
        {
            zoom_current += scroll * zoom_speed * Time.deltaTime;
            zoom_current = (zoom_current < zoom_ortho_min) ? zoom_ortho_min : zoom_current;
            zoom_current = (zoom_current > zoom_ortho_max) ? zoom_ortho_max : zoom_current;
            ref_camera.orthographicSize = zoom_current;
        }
    }
}
