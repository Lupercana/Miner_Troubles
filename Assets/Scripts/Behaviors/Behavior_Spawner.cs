using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct Node_Type
    {
        public GameObject ref_node;
        public int spawn_amount;
        public int spawn_chance;
        public int durability_multiplier;
    }
    [SerializeField]

    public struct Mining_Info
    {
        public int gem_tier;
        public int gem_amount;
        public float node_durability;

        public Mining_Info(int gt, int ga, float nd)
        {
            gem_tier = gt;
            gem_amount = ga;
            node_durability = nd;
        }
    }

    [SerializeField] private Node_Type[] node_types = null;
    [SerializeField] private Helper_Node_Durability script_durability_meter = null;

    [SerializeField] private ParticleSystem particle_mining = null;
    [SerializeField] private ParticleSystem particle_finished = null;
    [SerializeField] private ParticleSystem particle_lightning = null;
    [SerializeField] private Effect_Grow effect_explosion = null;
    [SerializeField] private SpriteRenderer effect_explosion_sprite_renderer = null;

    [SerializeField] private int mining_level_upgrade = 0; // After how many levels to upgrade gem tier
    [SerializeField] private float mining_particle_thresh = 0f;

    private ParticleSystem.MainModule MM_mining;
    private ParticleSystem.MainModule MM_finished;
    private ParticleSystem.TextureSheetAnimationModule TAM_finished;
    private float max_durability = 0f;
    private float current_durability = 0f;
    private float mining_residue = 0f;
    private float mining_particle_play_next;
    private int gem_tier = 0;
    private int gem_type = 0;
    private int total_type_chances = 0;
    private int current_amount = 0;
    private bool active = false;

    public Behavior_Node GetActiveNode()
    {
        if (active)
        {
            return node_types[gem_type].ref_node.GetComponent<Behavior_Node>();
        }

        return null;
    }

    public Mining_Info GetMiningInfo()
    {
        return new Mining_Info(gem_tier, current_amount, current_durability);
    }

    public void PlayParticleLightning()
    {
        particle_lightning.Play();
    }

    public void PlayEffectExplosion(float explosion_radius, Color explosion_color)
    {
        effect_explosion_sprite_renderer.color = explosion_color;
        effect_explosion.SetGrowDistance(explosion_radius);
        effect_explosion.Grow(explosion_radius);
    }

    public void DecreaseDurability(float durability_decrease) 
    {
        if (!active)
        {
            Debug.Log("Inactive Node Call");
            return;
        }

        // Decrease the durability of the node
        /* Now handled by Node script
        float durability_decrease = Time.deltaTime * Manager_Main.Instance.GetMiningLevel() * Manager_Main.Instance.GetToolSpeedup();
        */
        durability_decrease = (durability_decrease > current_durability) ? current_durability : durability_decrease; // Don't go over current durability
        current_durability -= durability_decrease;

        // Increase the gem gain from mining this node
        float amount_gain = durability_decrease / max_durability * current_amount;
        mining_residue += amount_gain;
        int gem_gain = (int)mining_residue;
        if (gem_gain > 0)
        {
            Manager_Main.Instance.ChangeGemQuantity(gem_tier, gem_gain);
            int xp_gain = Manager_Main.Instance.GetGemXP()[gem_tier] * gem_gain;
            Manager_Main.Instance.ChangeMiningXP(xp_gain);
            mining_residue -= gem_gain;
        }

        script_durability_meter.SetValue(current_durability / max_durability);
    }

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
        for (int i = 0; i <= gem_level && !found; ++i)
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
        for (int i = 0; i < node_types.Length && !found; ++i)
        {
            total += node_types[i].spawn_chance;
            if (val < total)
            {
                gem_type = i;
                found = true;
            }
        }

        max_durability = Manager_Main.Instance.GetGemHealths()[gem_tier] * node_types[gem_type].durability_multiplier;
        current_durability = max_durability;
        script_durability_meter.SetValue(current_durability / max_durability);
        node_types[gem_type].ref_node.SetActive(true);
        script_durability_meter.gameObject.SetActive(true);
        Color c = Manager_Main.Instance.GetGemColors()[gem_tier];
        SpriteRenderer node_sprite_renderer = node_types[gem_type].ref_node.GetComponentInChildren<SpriteRenderer>();
        node_sprite_renderer.color = c;
        current_amount = node_types[gem_type].spawn_amount;

        // Set particles
        MM_mining.startColor = c;
        MM_finished.startColor = c;
        TAM_finished.SetSprite(0, node_sprite_renderer.sprite);
            
        // Set variables
        active = true;
        mining_residue = 0.5f; // Solves rounding issues
        mining_particle_play_next = mining_particle_thresh;

        return true;
    }

    private void Awake()
    {
        MM_mining = particle_mining.main;
        MM_finished = particle_finished.main;
        TAM_finished = particle_finished.textureSheetAnimation;
    }

    private void Start()
    {
        foreach (Node_Type node in node_types)
        {
            total_type_chances += node.spawn_chance;
        }

        active = false;
    }

    private void Update()
    {
        if (active)
        {
            if (current_durability <= 0)
            {
                Manager_Sounds.Instance.PlayMiningFinished(GetActiveNode().GetSpriteRenderer().isVisible);

                active = false;
                node_types[gem_type].ref_node.SetActive(false);
                script_durability_meter.gameObject.SetActive(false);

                // Play effects
                particle_finished.Play();
            }
            else
            {
                float progress = 1f - (current_durability / max_durability);
                if (progress > mining_particle_play_next)
                {
                    particle_mining.Play();
                    Manager_Sounds.Instance.PlayMiningIntermediate(GetActiveNode().GetSpriteRenderer().isVisible);
                    mining_particle_play_next += mining_particle_thresh;
                }
            }
        }
    }
}
