using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Node : MonoBehaviour
{
    [SerializeField] private int spawn_amount = 1;
    [SerializeField] private int spawn_chance = 1;

    public int GetSpawnAmount() { return spawn_amount; }
    public int GetSpawnChance() { return spawn_chance; }
}
