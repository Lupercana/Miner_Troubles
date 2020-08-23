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

    public override void Activate()
    {
        if (unlockable && Manager_Main.Instance.GetGemQuantities()[cost_tier] >= cost_amount)
        {
            Manager_Main.Instance.GetGemQuantities()[cost_tier] -= cost_amount;
            script_unlock_room.gameObject.SetActive(true);
            script_opposite_gate.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    protected override void UpdateText()
    {
        if (unlockable)
        {
            Manager_Main.Instance.SetUIHelperText("Gate || Unlock cost:");
            int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
            helper_gems[cost_tier] = cost_amount;
            Manager_Main.Instance.SetUIHelperGems(helper_gems);
        }
        else
        {
            Manager_Main.Instance.SetUIHelperText("Permanent Gate");
            int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
            Manager_Main.Instance.SetUIHelperGems(helper_gems);
        }
    }
}
