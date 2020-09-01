using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Grow : MonoBehaviour
{
    [System.Serializable]
    public struct Axis
    {
        public bool use_x;
        public bool use_y;
        public bool use_z;
    }
    [SerializeField] private Axis axis = new Axis();

    [SerializeField] private SpriteRenderer ref_sprite_renderer = null;

    [SerializeField] private float growth_speed = 0f;
    [SerializeField] private float time_revert_seconds = 0f;
    [SerializeField] private float distance_revert = 0f;
    [SerializeField] private bool use_time = true;
    [SerializeField] private bool use_distance = false;
    [SerializeField] private bool set_inactive_after = false;

    private bool growing = false;
    private Vector3 initial_scale;
    private float growth_speed_multiplier = 1f;
    private float current_use_amount;
    private float start_time;

    public bool IsGrowing() { return growing; }

    public void SetGrowTime(float new_time) { time_revert_seconds = new_time; }
    public void SetGrowDistance(float new_distance) { distance_revert = new_distance; }

    public void Grow(float speed_mult = 0)
    {
        transform.localScale = initial_scale;
        ref_sprite_renderer.enabled = true;
        growing = true;
        growth_speed_multiplier = (speed_mult == 0) ? 1f : speed_mult;
        current_use_amount = 1;
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
        float average_size = ((axis.use_x ? sr_extents.x : 0) + (axis.use_y ? sr_extents.y : 0) + (axis.use_z ? sr_extents.z : 0)) / ((axis.use_x ? 1f : 0f) + (axis.use_y ? 1f : 0f) + (axis.use_z ? 1f : 0f));
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
            current_use_amount += growth_speed * growth_speed_multiplier * Time.deltaTime;

            float new_x = (axis.use_x ? current_use_amount : 1f) * initial_scale.x;
            float new_y = (axis.use_y ? current_use_amount : 1f) * initial_scale.y;
            float new_z = (axis.use_z ? current_use_amount : 1f) * initial_scale.z;
            transform.localScale = new Vector3(new_x, new_y, new_z);
        }
    }
}
