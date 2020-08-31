using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Gate : Behavior_Interactable
{
    [SerializeField] private Behavior_Room script_unlock_room = null;
    [SerializeField] private Behavior_Gate script_opposite_gate = null;

    [SerializeField] private bool unlockable = false;
    [SerializeField] private int cost_tier = 0;
    [SerializeField] private int cost_amount = 0;

    public override void Activate(Behavior_Seeker activator)
    {
        if (unlockable && Manager_Main.Instance.GetGemQuantities()[cost_tier] >= cost_amount)
        {
            Manager_Main.Instance.ChangeGemQuantity(cost_tier, -cost_amount);
            script_unlock_room.gameObject.SetActive(true);
            script_opposite_gate.gameObject.SetActive(false);
            gameObject.SetActive(false);

            // Play effects
            Manager_Sounds.Instance.PlayPurchase();
        }
        else
        {
            // Play effects
            Manager_Sounds.Instance.PlayDenied();
        }
    }

    protected override void UpdateText()
    {
        if (unlockable)
        {
            Manager_Main.Instance.SetUIHelperText("Gate || Unlock cost:");
            int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
            helper_gems[cost_tier] = cost_amount;
            Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
        }
        else
        {
            Manager_Main.Instance.SetUIHelperText("Permanent Gate");
            int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
            Manager_Main.Instance.SetUIHelperGems(false, helper_gems);
        }
    }
}
