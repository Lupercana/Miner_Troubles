using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Main : MonoBehaviour
{
    public enum Tool_Type
    {
        Pickaxe,
        Hammer,
        Bomb,
        Staff
    }

    [System.Serializable]
    public struct Tool
    {
        public Sprite sprite;
        public Tool_Type type;
        public int tier;
        public Tool(Sprite s, Tool_Type tt, int t)
        {
            sprite = s;
            type = tt;
            tier = t;
        }
    }

    public static Manager_Main Instance = null;

    // Cursor
    [SerializeField] public Texture2D cursor_normal = null;
    [SerializeField] public Texture2D cursor_interactable = null;
    [SerializeField] public Vector2 cursor_hotspot;

    // UI
    [SerializeField] private Image[] ui_gems = null;
    [SerializeField] private Text[] ui_gem_texts = null;
    [SerializeField] private GameObject ui_helper_base = null;
    [SerializeField] private Text ui_helper_general_text = null;
    [SerializeField] private Image[] ui_helper_gems = null;
    [SerializeField] private Text[] ui_helper_gem_texts = null;
    [SerializeField] private Text ui_text_mining_level = null;
    [SerializeField] private Slider ui_slider_mining_xp = null;
    [SerializeField] private Slider ui_slider_tool_cd = null;
    [SerializeField] private Image ui_image_slot_tool = null;
    [SerializeField] private Image ui_image_slot_ring = null;
    [SerializeField] private Image[] ui_image_slot_extra = null;

    // Particles
    [SerializeField] private ParticleSystem particle_level_up = null;

    [SerializeField] private Tool current_tool; // Modified later
    [SerializeField] private float[] tool_tier_speedups = null;
    [SerializeField] private Color[] gem_colors = null;
    [SerializeField] private float[] gem_healths = null;
    [SerializeField] private int[] gem_spawn_chance = null;
    [SerializeField] private int[] gem_xp = null;
    [SerializeField] private int[] gem_quantities = null; // Modified later
    [SerializeField] private Color max_mining_color = Color.white;
    [SerializeField] private int mining_level = 1; // Modified later
    [SerializeField] private int max_mining_level = 1;
    [SerializeField] private float leveling_exp_base = 1f;

    private int[] total_gem_chances;
    private int mining_xp = 0;
    private int xp_needed = 0;
    private int ui_helper_last_id = -1;

    public Color[] GetGemColors() { return gem_colors; }
    public float[] GetGemHealths() { return gem_healths; }
    public int[] GetGemQuantities() { return gem_quantities;  }
    public int[] GetGemXP() { return gem_xp; }
    public int GetMiningLevel() { return mining_level; }
    public int GetTotalGemChances(int gem_max) { return total_gem_chances[gem_max]; }
    public Tool GetCurrentTool() { return current_tool; }
    public float GetToolSpeedup() { return tool_tier_speedups[current_tool.tier]; }

    public void SetCursorNormal() { Cursor.SetCursor(cursor_normal, cursor_hotspot, CursorMode.Auto); }
    public void SetCursorInteractable() { Cursor.SetCursor(cursor_interactable, cursor_hotspot, CursorMode.Auto); }
    public void SetUIHelperActive(bool active, int caller_id)
    {
        if (active)
        {
            ui_helper_last_id = caller_id;
        }
        else if (caller_id != ui_helper_last_id)
        {
            return; // Another interactable is using the UI Helper
        }

        if (ui_helper_base)
        {
            ui_helper_base.SetActive(active);
            if (active)
            {
                SetCursorInteractable();
            }
            else
            {
                SetCursorNormal();
            }
        }
    }
    public void SetUIHelperGems(int[] gem_counts)
    {
        for (int i = 0; i < gem_counts.Length; ++i)
        {
            ui_helper_gem_texts[i].text = gem_counts[i].ToString();
        }
    }
    public void SetUIHelperText(string new_text)
    {
        ui_helper_general_text.text = new_text;
    }
    public void SetCurrentTool(Tool new_tool)
    {
        current_tool = new_tool;

        ui_image_slot_tool.sprite = current_tool.sprite;
        ui_image_slot_tool.color = gem_colors[current_tool.tier];
    }

    public void ChangeGemQuantity(int gem_tier, int change_amount)
    {
        if (gem_tier >= gem_quantities.Length)
        {
            Debug.Log("Invalid gem index");
            return;
        }

        gem_quantities[gem_tier] += change_amount;
        ui_gem_texts[gem_tier].text = gem_quantities[gem_tier].ToString();
    }
    public void ChangeMiningXP(int change_amount)
    {
        mining_xp += change_amount;
        while (mining_xp >= xp_needed && mining_level < max_mining_level)
        {
            mining_level += 1;
            mining_xp -= xp_needed;
            xp_needed = XPToNextLevel(mining_level);
            if (mining_level == max_mining_level)
            {
                // Just hit max level
                ui_text_mining_level.color = max_mining_color;
                ui_slider_mining_xp.value = 1f;
            }

            // Update UI
            ui_text_mining_level.text = "Lv " + mining_level;

            // Play effects
            particle_level_up.Play();
            Manager_Sounds.Instance.PlayLevelUp();
        }

        if (mining_level < max_mining_level)
        {
            ui_slider_mining_xp.value = (float)mining_xp / xp_needed;
        }
    }

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
        // Set cursor
        SetCursorNormal();

        // Set helper
        ui_helper_last_id = 0;
        SetUIHelperActive(false, ui_helper_last_id);

        // Set UI Slots
        SetCurrentTool(current_tool);

        // Set gem colors in UI
        for (int i = 0; i < ui_gems.Length; ++i)
        {
            ui_gems[i].color = gem_colors[i];
            ui_helper_gems[i].color = gem_colors[i];
            ui_gem_texts[i].text = gem_quantities[i].ToString();
        }

        // Set mining level text
        mining_level = (mining_level > max_mining_level) ? max_mining_level : mining_level;
        ui_text_mining_level.text = "Lv " + mining_level;

        // Set mining xp slider
        mining_xp = 0;
        xp_needed = XPToNextLevel(mining_level);
        ui_slider_mining_xp.value = (float)mining_xp / xp_needed;

        // Set mining tool CD slider
        ui_slider_tool_cd.value = 0;

        // Pre-calculate total gem chances
        int total = 0;
        total_gem_chances = new int[gem_spawn_chance.Length];
        for (int i = 0; i < gem_spawn_chance.Length; ++i)
        {
            total += gem_spawn_chance[i];
            total_gem_chances[i] = total;
        }
    }

    private int XPToNextLevel(int current_level)
    {
        return (int)Mathf.Pow(leveling_exp_base, current_level);
    }
}
