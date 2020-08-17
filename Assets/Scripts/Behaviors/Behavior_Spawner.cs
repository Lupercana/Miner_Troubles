using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Spawner : MonoBehaviour
{
    [SerializeField] private Behavior_Node[] ref_node_types = null;
    [SerializeField] private Behavior_Slider ref_durability_meter = null;

    [SerializeField] private int mining_level_upgrade = 0; // After how many levels to upgrade gem tier

    private float max_health = 0f;
    private float current_health = 0f;
    private int gem_tier;
    private int gem_type;
    private int total_type_chances = 0;
    private int current_amount = 0;
    private bool active;

    public bool Spawn()
    {
        if (active)
        {
            return false;
        }

        int mining_level = Manager_Main.Instance.GetMiningLevel();
        int max_gem_level = Manager_Main.Instance.GetGemColors().Length - 1;
        int gem_level = mining_level / mining_level_upgrade;
        gem_level = (gem_level > max_gem_level) ? max_gem_level : gem_level;

        gem_tier = 0;
        gem_type = 0;
        int val;
        bool found;
        // Choose a material to spawn
        int total_gem_chances = Manager_Main.Instance.GetTotalGemChances(gem_level);
        val = (int)(Random.value * total_gem_chances);
        val = (val >= total_gem_chances) ? (total_gem_chances - 1) : val;
        found = false;
        for (int i = 0; i < gem_level && !found; ++i)
        {
            if (val < Manager_Main.Instance.GetTotalGemChances(i))
            {
                gem_tier = i;
                found = true;
            }
        }

        // Choose a type to spawn
        val = (int)(Random.value * total_type_chances);
        val = (val >= total_type_chances) ? (total_type_chances - 1) : val;
        found = false;
        int total = 0;
        for (int i = 0; i < ref_node_types.Length && !found; ++i)
        {
            total += ref_node_types[i].GetSpawnChance();
            if (val < total)
            {
                gem_type = i;
                found = true;
            }
        }

        max_health = Manager_Main.Instance.GetGemHealths()[gem_tier];
        current_health = max_health;
        ref_node_types[gem_type].gameObject.SetActive(true);
        ref_durability_meter.gameObject.SetActive(true);
        ref_node_types[gem_type].GetComponent<SpriteRenderer>().color = Manager_Main.Instance.GetGemColors()[gem_tier];
        current_amount = ref_node_types[gem_type].GetSpawnAmount();

        active = true;
        return true;
    }

    private void Start()
    {
        foreach (Behavior_Node node in ref_node_types)
        {
            total_type_chances += node.GetSpawnChance();
        }

        active = false;
    }

    private void Update()
    {
        if (active)
        {
            current_health -= 1 * Time.deltaTime;
            ref_durability_meter.SetValue(current_health / max_health);
            if (current_health <= 0)
            {
                active = false;
                ref_node_types[gem_type].gameObject.SetActive(false);
                ref_durability_meter.gameObject.SetActive(false);
            }
        }
    }
}
