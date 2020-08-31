using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Node : Behavior_Interactable
{
    [SerializeField] private Behavior_Spawner script_parent_spawner = null;
    [SerializeField] private Effect_Grow script_effect_grow = null;
    [SerializeField] private Effect_Shake script_effect_shake = null;

    [SerializeField] private SpriteRenderer ref_self_sprite_renderer = null;

    private bool mining = false;
    private float mining_multiplier = 0f;
    private Manager_Main.Tool activate_tool;

    public Behavior_Spawner GetSpawner() { return script_parent_spawner; }
    public SpriteRenderer GetSpriteRenderer() { return ref_self_sprite_renderer; }

    public void Grow(float mult = 0)
    {
        if (!script_effect_grow.IsGrowing())
        {
            script_effect_grow.Grow(mult);
        }
    }

    public void Shake(float mult = 0)
    {
        script_effect_shake.Shake(mult);
    }

    public override void Activate(Behavior_Seeker activator)
    {
        activate_tool = activator.GetTool();

        mining = false;
        float base_mining_power = Time.deltaTime * Manager_Main.Instance.GetMiningLevel() * Manager_Main.Instance.GetToolSpeedup(activate_tool);
        switch (activate_tool.type)
        {
            case Manager_Main.Tool_Type.Gloves:
                mining_multiplier = Parameters_Mining.Instance.gloves_mining_multplier;
                mining = true;
                Manager_Sounds.Instance.PlayBasicHit(ref_self_sprite_renderer.isVisible);
                break;
            case Manager_Main.Tool_Type.Pickaxe:
                mining_multiplier = Parameters_Mining.Instance.pickaxe_mining_multplier;
                mining = true;
                Manager_Sounds.Instance.PlayBasicHit(ref_self_sprite_renderer.isVisible);
                break;
            case Manager_Main.Tool_Type.Hammer:
                mining_multiplier = Parameters_Mining.Instance.hammer_mining_multplier;
                script_parent_spawner.DecreaseDurability(base_mining_power * mining_multiplier);
                Grow(Parameters_Mining.Instance.hammer_grow_mult);
                Shake(Parameters_Mining.Instance.hammer_shake_mult);
                Manager_Sounds.Instance.PlayHammerHit(ref_self_sprite_renderer.isVisible);
                break;
            case Manager_Main.Tool_Type.Bomb:
                mining_multiplier = Parameters_Mining.Instance.bomb_mining_multplier;
                float explosion_radius = Parameters_Mining.Instance.bomb_radius + Manager_Main.Instance.GetMiningLevel() * Parameters_Mining.Instance.bomb_mine_level_scale;
                Collider2D[] nearby_objs_colli = Physics2D.OverlapCircleAll(transform.position, explosion_radius);
                foreach(Collider2D colli in nearby_objs_colli)
                {
                    if (colli.tag == "Node")
                    {
                        Behavior_Node script_node = colli.GetComponent<Behavior_Node>();
                        script_node.GetSpawner().DecreaseDurability(base_mining_power * mining_multiplier);
                        script_node.Grow();
                    }
                }
                script_parent_spawner.PlayEffectExplosion(explosion_radius, Manager_Main.Instance.GetGemColors()[activate_tool.tier]);
                Manager_Sounds.Instance.PlayBombHit(ref_self_sprite_renderer.isVisible);
                break;
            case Manager_Main.Tool_Type.Staff:
                mining_multiplier = Parameters_Mining.Instance.staff_mining_multplier;
                mining = true;
                Manager_Sounds.Instance.PlayStaffHit(ref_self_sprite_renderer.isVisible);
                break;
        }
    }

    public override void Deactivate()
    {
        mining = false;
    }

    protected override void UpdateText()
    {
        Behavior_Spawner.Mining_Info mi = script_parent_spawner.GetMiningInfo();
        Manager_Main.Instance.SetUIHelperText("Mining Node || Contains:");
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        helper_gems[mi.gem_tier] = mi.gem_amount;
        Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mining)
        {
            float base_mining_power = Time.deltaTime * Manager_Main.Instance.GetMiningLevel() * Manager_Main.Instance.GetToolSpeedup(activate_tool);
            
            if (activate_tool.type == Manager_Main.Tool_Type.Staff)
            {
                // Chaining
                HashSet<int> processed_targets = new HashSet<int>();
                Queue<Behavior_Spawner> targets = new Queue<Behavior_Spawner>();
                targets.Enqueue(script_parent_spawner);

                while (targets.Count > 0)
                {
                    Behavior_Spawner target = targets.Dequeue();
                    target.DecreaseDurability(base_mining_power * mining_multiplier);
                    target.PlayParticleLightning();
                    processed_targets.Add(target.gameObject.GetInstanceID());

                    Collider2D[] nearby_objs_colli = Physics2D.OverlapCircleAll(target.gameObject.transform.position, Parameters_Mining.Instance.staff_chain_radius + Manager_Main.Instance.GetMiningLevel() * Parameters_Mining.Instance.staff_mine_level_scale);
                    foreach (Collider2D colli in nearby_objs_colli)
                    {
                        if (colli.tag == "Node")
                        {
                            Behavior_Node new_node = colli.GetComponent<Behavior_Node>();
                            Behavior_Spawner new_spawner = new_node.GetSpawner();

                            if (!processed_targets.Contains(new_spawner.gameObject.GetInstanceID()))
                            {
                                // Unprocessed Target
                                targets.Enqueue(new_spawner);
                                new_node.Grow();
                            }
                        }
                    }
                }
            }
            else
            {
                script_parent_spawner.DecreaseDurability(base_mining_power * mining_multiplier);
            }

            // Play effect
            Grow();
        }
    }

    private void OnDisable()
    {
        DisableHelperText();
    }
}
