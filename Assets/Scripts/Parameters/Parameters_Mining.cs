using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Mining : MonoBehaviour
{
    public static Parameters_Mining Instance;

    [SerializeField] public float gloves_mining_multiplier = 1f;
    [SerializeField] public float pickaxe_mining_multiplier = 1f;
    [SerializeField] public float hammer_mining_multiplier = 1f;
    [SerializeField] public float hammer_grow_mult = 1f;
    [SerializeField] public float hammer_shake_mult = 1f;
    [SerializeField] public float bomb_mining_multiplier = 1f;
    [SerializeField] public float bomb_radius = 1f;
    [SerializeField] public float bomb_mine_level_scale = 0.1f;
    [SerializeField] public float staff_mining_multiplier = 1f;
    [SerializeField] public float staff_chain_radius = 1f;
    [SerializeField] public float staff_mine_level_scale = 0.01f;
    [SerializeField] public float torch_mining_multiplier = 1f;
    [SerializeField] public float torch_burn_duration_seconds = 1f;
    [SerializeField] public float torch_duration_level_scale = 0.1f;

    private void Awake()
    {
        Instance = this;
    }
}
