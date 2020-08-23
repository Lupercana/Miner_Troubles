using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Workbench : Behavior_Interactable
{
    public override void Activate()
    {

    }

    protected override void UpdateText()
    {
        Manager_Main.Instance.SetUIHelperText("Workbench || Upgrade cost:");
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        Manager_Main.Instance.SetUIHelperGems(helper_gems);
    }
}
