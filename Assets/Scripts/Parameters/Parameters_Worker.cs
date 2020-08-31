using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Worker : MonoBehaviour
{
    public static Parameters_Worker Instance;

    [SerializeField] public Manager_Main.Gem_Cost activate_cost = new Manager_Main.Gem_Cost();
    [SerializeField] public float[] move_speeds = null;
    [SerializeField] public float[] activate_delay_seconds = null;
    [SerializeField] public float stuck_velocity_threshold = 0f;
    [SerializeField] public float stuck_timeout_seconds = 1f;

    private void Awake()
    {
        Instance = this;
    }
}
