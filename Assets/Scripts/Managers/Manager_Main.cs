using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Main : MonoBehaviour
{
    public static Manager_Main Instance = null;

    [SerializeField] private Image[] ui_gems = null;
    [SerializeField] private Text[] ui_gem_texts = null;
    [SerializeField] private Text ui_text_mining_level = null;

    [SerializeField] private Color[] gem_colors = null;
    [SerializeField] private float[] gem_healths = null;
    [SerializeField] private int[] gem_spawn_chance = null;
    [SerializeField] private int[] gem_quantities = null; // Modified later
    [SerializeField] private int mining_level = 1; // Modified later
    [SerializeField] private int max_mining_level = 1;

    private int[] total_gem_chances;

    public Color[] GetGemColors() { return gem_colors; }
    public float[] GetGemHealths() { return gem_healths; }
    public int GetGemQuantity(int gem_index)
    {
        if (gem_index >= gem_quantities.Length)
        {
            Debug.Log("Invalid gem index");
            return 0;
        }

        return gem_quantities[gem_index]; 
    }
    public int GetMiningLevel() { return mining_level; }
    public int GetTotalGemChances(int gem_max) { return total_gem_chances[gem_max]; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Set gem colors in UI
        for (int i = 0; i < ui_gems.Length; ++i)
        {
            ui_gems[i].color = gem_colors[i];
            ui_gem_texts[i].text = gem_quantities[i].ToString();
        }

        // Set mining level text
        mining_level = (mining_level > max_mining_level) ? max_mining_level : mining_level;
        ui_text_mining_level.text = "Lv " + mining_level;

        // Pre-calculate total gem chances
        int total = 0;
        total_gem_chances = new int[gem_spawn_chance.Length];
        for (int i = 0; i < gem_spawn_chance.Length; ++i)
        {
            total += gem_spawn_chance[i];
            total_gem_chances[i] = total;
        }
    }
}
