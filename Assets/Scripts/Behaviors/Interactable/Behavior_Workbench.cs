using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Workbench : Behavior_Interactable
{
    [SerializeField] private Manager_Main.Tool_Type workshop_type = Manager_Main.Tool_Type.Pickaxe;

    [SerializeField] private int workshop_tool_upgrade_cost = 0; // Of the current tool tier gems
    [SerializeField] private int workshop_tool_change_cost = 0; // Of the current tool tier gems

    public override void Activate()
    {
        Manager_Main.Tool current_tool = Manager_Main.Instance.slot_tool.GetTool();
        int cost = (workshop_type == current_tool.type) ? workshop_tool_upgrade_cost : workshop_tool_change_cost;

        if (Manager_Main.Instance.GetGemQuantities()[current_tool.tier] >= cost && (current_tool.tier < Manager_Main.Instance.GetGemColors().Length - 1 || workshop_type != current_tool.type))
        {
            // Take payment
            Manager_Main.Instance.ChangeGemQuantity(current_tool.tier, -cost);

            int new_tier = (workshop_type == current_tool.type) ? current_tool.tier + 1 : current_tool.tier;
            // Create new tool
            Manager_Main.Tool new_tool = new Manager_Main.Tool(workshop_type, new_tier);
            Manager_Main.Instance.slot_tool.SetTool(new_tool);

            UpdateText(); // In case mouse is still over

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
        Manager_Main.Tool current_tool = Manager_Main.Instance.slot_tool.GetTool();

        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        if (current_tool.type == workshop_type) // Upgrade tool
        {
            if (current_tool.tier < Manager_Main.Instance.GetGemColors().Length - 1) // Can still upgrade
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop\nNext upgrade cost:");
                helper_gems[current_tool.tier] = workshop_tool_upgrade_cost;
                Manager_Main.Instance.SetUIHelperGems(helper_gems);
            }
            else // No more upgrades
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop\nNo more upgrades");
                Manager_Main.Instance.SetUIHelperGems(helper_gems);
            }

        }
        else // Change tool
        {
            Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop\nTool change cost:");
            helper_gems[current_tool.tier] = workshop_tool_change_cost;
            Manager_Main.Instance.SetUIHelperGems(helper_gems);
        }
    }
}
