using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Interactables : MonoBehaviour
{
    public static Parameters_Interactables Instance;

    // Workbench
    [SerializeField] public int[] workbench_upgrade_costs = null; // Cost of being at this tier and going to the next (So there should be 1 less than number of gem colors)

    // Worker
    [SerializeField] public float[] worker_move_speeds = null;
    [SerializeField] public float[] worker_activate_delay_seconds = null;
    [SerializeField] public float worker_stuck_velocity_threshold = 0f;
    [SerializeField] public float worker_stuck_timeout_seconds = 1f;

    private void Awake()
    {
        Instance = this;
    }
}
