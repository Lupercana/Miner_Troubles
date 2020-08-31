using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Workbench : Behavior_Interactable
{
    [SerializeField] private Manager_Main.Tool_Type workshop_type = Manager_Main.Tool_Type.Pickaxe;
    [SerializeField] private string workshop_text = "";
    [SerializeField] private int workshop_tool_upgrade_cost = 0; // Of the current tool tier gems
    [SerializeField] private Manager_Main.Gem_Cost initial_cost = new Manager_Main.Gem_Cost();

    public override void Activate(Behavior_Seeker activator)
    {
        bool upgrade;
        Manager_Main.Slot free_slot = new Manager_Main.Slot();

        Manager_Main.Slot existing_tool_slot = FindExistingSlot();
        Manager_Main.Gem_Cost cost = new Manager_Main.Gem_Cost();
        if (existing_tool_slot != null) // Upgrade
        {
            if (existing_tool_slot.GetTool().tier >= Manager_Main.Instance.GetGemColors().Length - 1)
            {
                // Play effects
                Manager_Sounds.Instance.PlayDenied(true);
                return;
            }

            upgrade = true;

            cost.gem_tier = existing_tool_slot.GetTool().tier;
            cost.gem_amount = workshop_tool_upgrade_cost;
        }
        else // Add tool
        {
            upgrade = false;
            free_slot = FindFreeSlot();
            if (free_slot == null)
            {
                // Play effects
                Manager_Sounds.Instance.PlayDenied(true);
                return;
            }

            // There are free slots
            cost = initial_cost;
        }

        if (Manager_Main.Instance.GetGemQuantities()[cost.gem_tier] >= cost.gem_amount)
        {
            // Take payment
            Manager_Main.Instance.ChangeGemQuantity(cost.gem_tier, -cost.gem_amount);

            if (upgrade)
            {
                existing_tool_slot.SetTool(new Manager_Main.Tool(workshop_type, cost.gem_tier + 1));
            }
            else // Add
            {
                free_slot.SetTool(new Manager_Main.Tool(workshop_type, 0));
            }

            UpdateText(); // In case mouse is still over

            // Play effects
            Manager_Sounds.Instance.PlayPurchase(true);
        }
        else // Not enough gems
        {
            // Play effects
            Manager_Sounds.Instance.PlayDenied(true);
        }
    }

    protected override void UpdateText()
    {
        Manager_Main.Slot existing_tool_slot = FindExistingSlot();

        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        if (existing_tool_slot != null) // Upgrade tool
        {
            Manager_Main.Tool current_tool = existing_tool_slot.GetTool();
            if (current_tool.tier < Manager_Main.Instance.GetGemColors().Length - 1) // Can still upgrade
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop " + workshop_text + "\nNext upgrade cost:");
                helper_gems[current_tool.tier] = workshop_tool_upgrade_cost;
                Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
            }
            else // No more upgrades
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop " + workshop_text + "\nNo more upgrades");
                Manager_Main.Instance.SetUIHelperGems(false, helper_gems);
            }

        }
        else // Change tool
        {
            if (FindFreeSlot() != null)
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop " + workshop_text + "\nTool cost:");
                helper_gems[initial_cost.gem_tier] = initial_cost.gem_amount;
                Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
            }
            else
            {
                Manager_Main.Instance.SetUIHelperText(Manager_Main.Instance.GetToolName(workshop_type) + " (" + Manager_Main.Instance.GetToolDescription(workshop_type) + ") " + " Workshop " + workshop_text + "\nNo more slot space");
                Manager_Main.Instance.SetUIHelperGems(false, helper_gems);
            }
        }
    }

    private Manager_Main.Slot FindExistingSlot()
    {
        Manager_Main.Slot existing_tool_slot = null;
        // Search equipment for workshop type
        if (Manager_Main.Instance.slot_tool.GetTool().type == workshop_type)
        {
            existing_tool_slot = Manager_Main.Instance.slot_tool;
        }
        else
        {
            foreach (Manager_Main.Slot slot in Manager_Main.Instance.slot_extras)
            {
                if (slot.GetTool().type == workshop_type)
                {
                    existing_tool_slot = slot;
                }
            }
        }

        return existing_tool_slot;
    }

    private Manager_Main.Slot FindFreeSlot()
    {
        foreach (Manager_Main.Slot slot in Manager_Main.Instance.slot_extras)
        {
            if (slot.GetTool().type == Manager_Main.Tool_Type.None)
            {
                return slot;
            }
        }

        return null;
    }
}
