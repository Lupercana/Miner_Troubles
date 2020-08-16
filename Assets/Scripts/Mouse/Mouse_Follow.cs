using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Follow : MonoBehaviour
{
    private void Update()
    {
        Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse_world.x, mouse_world.y);
    }
}
