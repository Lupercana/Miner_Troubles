using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Cursor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ref_sprite_renderer = null;
    [SerializeField] private Sprite image_cursor = null;
    [SerializeField] private Vector2 hotspot = Vector2.zero; // Top left corner

    private void Awake()
    {
        Cursor.visible = false;
        ref_sprite_renderer.sprite = image_cursor;
    }

    private void Update()
    {
        Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse_world.x - hotspot.x, mouse_world.y + hotspot.y, 0f);
    }
}
