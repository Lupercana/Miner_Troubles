using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoom : MonoBehaviour
{
    [SerializeField] private Camera ref_camera = null;
    [SerializeField] private float zoom_ortho_min = 0f;
    [SerializeField] private float zoom_ortho_max = 0f;
    [SerializeField] private float zoom_speed_mouse = 0f;
    [SerializeField] private float zoom_speed_keyboard = 0f;

    private float zoom_current;

    private void Start()
    {
        ref_camera.orthographic = true;
        zoom_current = ref_camera.orthographicSize;
    }

    private void Update()
    {
        float scroll_mouse = Input.GetAxis("Mouse ScrollWheel") * -1 * zoom_speed_mouse;
        float scroll_keyboard = ((Input.GetButton("Zoom_In") ? -1f : 0) + (Input.GetButton("Zoom_Out") ? 1f : 0)) * zoom_speed_keyboard;
        float scroll = (scroll_mouse == 0f) ? scroll_keyboard : scroll_mouse;
        if (scroll != 0f)
        {
            zoom_current += scroll * Time.deltaTime;
            zoom_current = (zoom_current < zoom_ortho_min) ? zoom_ortho_min : zoom_current;
            zoom_current = (zoom_current > zoom_ortho_max) ? zoom_ortho_max : zoom_current;
            ref_camera.orthographicSize = zoom_current;
        }
    }
}
