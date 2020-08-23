using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Node : Behavior_Interactable
{
    [SerializeField] private Behavior_Spawner ref_parent_spawner = null;

    [SerializeField] private int spawn_amount = 1;
    [SerializeField] private int spawn_chance = 1;

    public int GetSpawnAmount() { return spawn_amount; }
    public int GetSpawnChance() { return spawn_chance; }

    public override void Activate()
    {
        ref_parent_spawner.SetMining(true);
    }

    public override void Deactivate()
    {
        ref_parent_spawner.SetMining(false);
    }

    protected override void UpdateText()
    {
        Behavior_Spawner.Mining_Info mi = ref_parent_spawner.GetMiningInfo();
        Manager_Main.Instance.SetUIHelperText("Mining Node || Contains:");
        int[] helper_gems = new int[Manager_Main.Instance.GetGemColors().Length];
        helper_gems[mi.gem_tier] = mi.gem_amount;
        Manager_Main.Instance.SetUIHelperGems(helper_gems);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        DisableHelperText();
    }
}
