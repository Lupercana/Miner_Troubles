using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Upverter : Behavior_Interactable
{
    [SerializeField] private Effect_Shake script_effect_shake = null;

    [SerializeField] private SpriteRenderer ref_inside_sprite_renderer = null;

    [SerializeField] private Manager_Main.Gem_Cost cost = new Manager_Main.Gem_Cost();
    [SerializeField] private float gain_ratio = 1f;

    public override void Activate(Behavior_Seeker activator)
    {
        if (Manager_Main.Instance.GetGemQuantities()[cost.gem_tier] >= cost.gem_amount)
        {
            // Convert gems
            Manager_Main.Instance.ChangeGemQuantity(cost.gem_tier, -cost.gem_amount);
            Manager_Main.Instance.ChangeGemQuantity(cost.gem_tier + 1, Mathf.FloorToInt(cost.gem_amount * gain_ratio));

            // Play effects
            script_effect_shake.Shake();
            Manager_Sounds.Instance.PlayUpverter(true);
        }
        else
        {
            // Play effects
            Manager_Sounds.Instance.PlayDenied(true);
        }
    }

    protected override void UpdateText()
    {
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        Manager_Main.Instance.SetUIHelperText("Gem Upverter || Converts gems into higher tier gems at a fixed ratio\nRatio: " + gain_ratio + " || Cost: ");
        helper_gems[cost.gem_tier] = cost.gem_amount;
        Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
    }

    private void Start()
    {
        ref_inside_sprite_renderer.color = Manager_Main.Instance.GetGemColors()[cost.gem_tier + 1];
    }
}
