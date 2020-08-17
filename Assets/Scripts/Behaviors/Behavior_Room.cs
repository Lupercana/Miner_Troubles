using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Room : MonoBehaviour
{
    [SerializeField] private GameObject spawners = null; // Structure: Spawners/(Rows * X)/(Spawner * Y), total spawners = X * Y

    [SerializeField] private float spawn_time_seconds = 0f; // A node will be prompted to spawn every this_value seconds divided by mining level in THIS room
    [SerializeField] private float spawn_time_deviation_seconds = 0f; // Deviation for spawn time in THIS room

    private List<Behavior_Spawner> scripts_spawner = new List<Behavior_Spawner>();
    private float last_spawn_time;
    private float spawn_time_wait;
    private int active_nodes;

    private void Awake()
    {
        // Get all spawner scripts
        foreach (Transform t_child in spawners.transform)
        {
            foreach (Transform t_grandchildren in t_child)
            {
                scripts_spawner.Add(t_grandchildren.GetComponent<Behavior_Spawner>());
            }
        }
    }

    private void Start()
    {
        active_nodes = 0;
        last_spawn_time = Time.time;
        spawn_time_wait = 0;
    }

    private void Update()
    {
        float elapsed_time = Time.time - last_spawn_time;
        if (elapsed_time > spawn_time_wait)
        {
            int mining_level = Manager_Main.Instance.GetMiningLevel();

            // Spawn rates slow down as nodes fill up the room, since requests to active spawners will fail
            int random_spawner = Random.Range(0, scripts_spawner.Count);
            random_spawner = (random_spawner >= scripts_spawner.Count) ? (scripts_spawner.Count - 1) : random_spawner;
            if (scripts_spawner[random_spawner].Spawn()) // Returns true if successfully just spawned
            {
                ++active_nodes;
            }

            float level_effect = mining_level / 10f + 1;
            spawn_time_wait = Random.Range((spawn_time_seconds - spawn_time_deviation_seconds) / (level_effect), (spawn_time_seconds + spawn_time_deviation_seconds) / (level_effect));
            last_spawn_time = Time.time;
        }
    }
}
