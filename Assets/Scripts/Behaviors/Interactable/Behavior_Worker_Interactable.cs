using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Worker_Interactable : Behavior_Interactable
{
    [SerializeField] private Behavior_Worker ref_script_worker = null;

    public override void Activate(Behavior_Seeker activator)
    {
        ref_script_worker.Activate();
    }

    protected override void UpdateText()
    {
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        Manager_Main.Instance.SetUIHelperText("");
        Manager_Main.Instance.SetUIHelperGems(false);
    }
}
