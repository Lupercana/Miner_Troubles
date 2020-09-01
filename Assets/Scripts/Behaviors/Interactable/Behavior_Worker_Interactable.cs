using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Worker_Interactable : Behavior_Interactable
{
    [SerializeField] private Behavior_Worker script_worker = null;
    [SerializeField] private Effect_Shake script_effect_shake = null;
    [SerializeField] private Effect_Grow script_effect_grow = null;

    [SerializeField] private Manager_Main.Gem_Cost cost = new Manager_Main.Gem_Cost();

    bool activated = false;

    public override void Activate(Behavior_Seeker activator)
    {
        if (!activated)
        {
            ActivateWorker();
        }
        else
        {
            TradeWorker();
        }
    }

    protected override void UpdateText()
    {
        if (!activated)
        {
            int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
            Manager_Main.Instance.SetUIHelperText("Worker || Becomes more effective with tool tier\nTool: " + Manager_Main.Instance.GetToolName(script_worker.GetTool().type) + " || Activation cost: ");
            helper_gems[cost.gem_tier] = cost.gem_amount;
            Manager_Main.Instance.SetUIHelperGems(true, helper_gems);
        }
        else
        {
            Manager_Main.Instance.SetUIHelperText("Worker || Becomes more effective with tool tier\nTool: " + Manager_Main.Instance.GetToolName(script_worker.GetTool().type) + " || Can trade tools");
            Manager_Main.Instance.SetUIHelperGems(false);
        }
    }

    private void ActivateWorker()
    {
        if (Manager_Main.Instance.GetGemQuantities()[cost.gem_tier] >= cost.gem_amount)
        {
            // Take payment
            Manager_Main.Instance.ChangeGemQuantity(cost.gem_tier, -cost.gem_amount);

            script_worker.Activate();
            activated = true;

            //Play effects
            Manager_Sounds.Instance.PlayWorkerActivate(true);
            script_effect_shake.Shake();
        }
        else
        {
            // Play effects
            Manager_Sounds.Instance.PlayDenied(true);
        }
    }

    private void TradeWorker()
    {
        Manager_Main.Tool temp = Manager_Main.Instance.slot_tool.GetTool();
        Manager_Main.Instance.slot_tool.SetTool(script_worker.GetTool()) ;
        script_worker.SetTool(temp);

        // Play effects
        script_effect_grow.Grow();
        Manager_Sounds.Instance.PlayWorkerToolSwap(true);
    }
}
