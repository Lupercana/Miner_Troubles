using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Grow : MonoBehaviour
{
    [System.Serializable]
    public struct Scale
    {
        public bool scale_x;
        public bool scale_y;
        public bool scale_z;
    }

    [SerializeField] private SpriteRenderer ref_sprite_renderer = null;

    [SerializeField] private float growth_speed = 0f;
    [SerializeField] private float time_revert_seconds = 0f;
    [SerializeField] private float distance_revert = 0f;
    [SerializeField] private bool use_time = true;
    [SerializeField] private bool use_distance = false;
    [SerializeField] private bool set_inactive_after = false;
    [SerializeField] private Scale scale = new Scale();

    private bool growing = false;
    private Vector3 initial_scale;
    private float growth_speed_multiplier = 1f;
    private float current_scale_amount;
    private float start_time;

    public void SetGrowSpeedMultplier(float new_mult) { growth_speed_multiplier = new_mult; }
    public void SetGrowTime(float new_time) { time_revert_seconds = new_time; }
    public void SetGrowDistance(float new_distance) { distance_revert = new_distance; }

    public void Grow()
    {
        ref_sprite_renderer.enabled = true;
        growing = true;
        current_scale_amount = 1;
        start_time = Time.time;
    }

    private void Start()
    {
        initial_scale = transform.localScale;
    }

    private void Update()
    {
        if (!growing)
        {
            return;
        }

        float e_time = Time.time - start_time;
        Vector3 sr_extents = ref_sprite_renderer.bounds.extents;
        float average_size = ((scale.scale_x ? sr_extents.x : 0) + (scale.scale_y ? sr_extents.y : 0) + (scale.scale_z ? sr_extents.z : 0)) / ((scale.scale_x ? 1f : 0f) + (scale.scale_y ? 1f : 0f) + (scale.scale_z ? 1f : 0f));
        if ((use_time && e_time >= time_revert_seconds) || (use_distance && average_size >= distance_revert))
        {
            // Revert
            transform.localScale = initial_scale;
            growing = false;
            if (set_inactive_after)
            {
                ref_sprite_renderer.enabled = false;
            }
        }
        else
        {
            current_scale_amount += growth_speed * growth_speed_multiplier * Time.deltaTime;

            float new_x = (scale.scale_x ? current_scale_amount : 1f) * initial_scale.x;
            float new_y = (scale.scale_y ? current_scale_amount : 1f) * initial_scale.y;
            float new_z = (scale.scale_z ? current_scale_amount : 1f) * initial_scale.z;
            transform.localScale = new Vector3(new_x, new_y, new_z);
        }
    }
}
