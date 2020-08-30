using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Shake : MonoBehaviour
{
    [System.Serializable]
    public struct Axis
    {
        public bool use_x;
        public bool use_y;
        public bool use_z;
    }
    [SerializeField] private Axis axis = new Axis();

    [SerializeField] private float shake_strength = 0.1f;
    [SerializeField] private float shake_duration_seconds = 0.1f;
    [SerializeField] private bool fade = true;

    private Vector3 original_pos;
    private float shake_time_start = 0f;
    private float shake_multiplier = 1f;
    private bool shake = false;

    public bool IsShaking() { return shake; }

    public void Shake(float new_shake_multiplier = 0)
    {
        shake_multiplier = (new_shake_multiplier == 0) ? 1f : new_shake_multiplier;
        shake = true;
        shake_time_start = Time.time;
    }

    private void Start()
    {
        shake_time_start = 0;
        shake = false;
        original_pos = transform.localPosition;
    }

    private void Update()
    {
        float e_time = Time.time - shake_time_start;
        if (shake && (e_time) < shake_duration_seconds)
        {
            float shake_amount = shake_strength * shake_multiplier * (fade ? (1f - e_time / shake_duration_seconds) : 1f);
            float offset_x = Random.Range(-shake_amount, shake_amount);
            float offset_y = Random.Range(-shake_amount, shake_amount);
            float offset_z = Random.Range(-shake_amount, shake_amount);

            transform.localPosition = new Vector3(
                original_pos.x + (axis.use_x ? offset_x : 0f),
                original_pos.y + (axis.use_y ? offset_y : 0f),
                original_pos.z + (axis.use_z ? offset_z : 0f));
        }
        else
        {
            shake = false;
            transform.localPosition = original_pos;
        }
    }
}
