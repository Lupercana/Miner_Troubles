using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Worker_Interactable : Behavior_Interactable
{
    [SerializeField] private Behavior_Worker script_worker = null;
    [SerializeField] private Effect_Shake script_effect_shake = null;

    public override void Activate(Behavior_Seeker activator)
    {
        script_worker.Activate();

        //Play effects
        Manager_Sounds.Instance.PlayWorkerActivate(true);
        script_effect_shake.Shake();
    }

    protected override void UpdateText()
    {
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        Manager_Main.Instance.SetUIHelperText("");
        Manager_Main.Instance.SetUIHelperGems(false);
    }
}
