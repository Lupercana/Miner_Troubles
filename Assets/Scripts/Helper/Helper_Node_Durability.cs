using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper_Node_Durability : MonoBehaviour
{
    [SerializeField] private GameObject ref_fill = null;

    private SpriteRenderer ref_fill_sprite_rend = null;
    private float local_scale_x;
    private float value = 0f;

    public float GetValue() { return value; }
    public void SetValue(float v)
    {
        value = Mathf.Clamp(v, 0, 1);
        float image_width = ref_fill_sprite_rend.bounds.size.x;
        ref_fill.transform.localScale = new Vector3(value * local_scale_x, ref_fill.transform.localScale.y, ref_fill.transform.localScale.z);
        float displacement_x = image_width - ref_fill_sprite_rend.bounds.size.x;
        ref_fill.transform.localPosition -= new Vector3(displacement_x / 2, 0, 0);
    }

    private void Awake()
    {
        ref_fill_sprite_rend = ref_fill.GetComponent<SpriteRenderer>();

        local_scale_x = ref_fill.transform.localScale.x;
    }
}
