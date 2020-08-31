using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Main : MonoBehaviour
{
    public static Manager_Main Instance = null;

    public enum Tool_Type
    {
        None,
        Gloves,
        Pickaxe,
        Hammer,
        Bomb,
        Staff
    }

    [System.Serializable]
    public struct Tool_Map
    {
        public Tool_Type tool;
        public Sprite sprite;
        public string name;
        public string description;
    }
    [SerializeField] private Tool_Map[] tool_map = null;
    private Dictionary<Tool_Type, Sprite> TT_Sprite = new Dictionary<Tool_Type, Sprite>(); // Built in awake
    private Dictionary<Tool_Type, string> TT_Name = new Dictionary<Tool_Type, string>(); // Built in awake
    private Dictionary<Tool_Type, string> TT_Description = new Dictionary<Tool_Type, string>(); // Built in awake

    [System.Serializable]
    public class Tool
    {
        public Tool_Type type;
        public int tier;
        public Tool()
        {
            type = Tool_Type.None;
            tier = 0;
        }
        public Tool(Tool_Type tt, int t)
        {
            type = tt;
            tier = t;
        }
    }

    [System.Serializable]
    public class Slot
    {
        [SerializeField] private Image ref_image = null;
        [SerializeField] private Tool current_tool;

        public Tool GetTool() { return current_tool; }

        public void SetTool(Tool new_tool)
        {
            current_tool = new_tool;
            ref_image.sprite = Manager_Main.Instance.GetToolSprite(new_tool.type);
            ref_image.color = Manager_Main.Instance.GetGemColors()[current_tool.tier];
        }
    }

    [System.Serializable]
    public struct Gem_Cost
    {
        public int gem_tier;
        public int gem_amount;
    }

    // Cursor
    [SerializeField] public Texture2D cursor_normal = null;
    [SerializeField] public Texture2D cursor_interactable = null;
    [SerializeField] public Vector2 cursor_hotspot;

    // UI
    [SerializeField] private Image[] ui_gems = null;
    [SerializeField] private Text[] ui_gem_texts = null;
    [SerializeField] private UI_Transition_Slide ui_helper_base = null;
    [SerializeField] private Text ui_helper_general_text = null;
    [SerializeField] private Image[] ui_helper_gems = null;
    [SerializeField] private Text[] ui_helper_gem_texts = null;
    [SerializeField] private Text ui_text_mining_level = null;
    [SerializeField] private Slider ui_slider_mining_xp = null;
    [SerializeField] private Slider ui_slider_tool_cd = null;

    // Slots
    [SerializeField] public Slot slot_tool = null;
    [SerializeField] public Slot slot_ring = null;
    [SerializeField] public Slot[] slot_extras = null;

    // Effects
    [SerializeField] private ParticleSystem particle_level_up = null;

    // Misc
    [SerializeField] private Behavior_Player ref_player = null;
    [SerializeField] private float[] tool_tier_speedups = null;
    [SerializeField] private Color[] gem_colors = null;
    [SerializeField] private float[] gem_healths = null;
    [SerializeField] private int[] gem_spawn_chance = null;
    [SerializeField] private int[] gem_xp = null;
    [SerializeField] private int[] gem_quantities = null; // Modified later
    [SerializeField] private Color maxed_color = Color.white;
    [SerializeField] private int mining_level = 1; // Modified later
    [SerializeField] private int max_mining_level = 1;
    [SerializeField] private int max_gems = 1;
    [SerializeField] private float leveling_exp_base = 1f;

    private Color original_gem_text_color;
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
    public float GetToolSpeedup(Tool tool) { return tool_tier_speedups[tool.tier]; }
    public Sprite GetToolSprite(Tool_Type tool_type) { return TT_Sprite[tool_type]; }
    public string GetToolName(Tool_Type tool_type) { return TT_Name[tool_type]; }
    public string GetToolDescription(Tool_Type tool_type) { return TT_Description[tool_type]; }

    public void SetCursorNormal() { Cursor.SetCursor(cursor_normal, cursor_hotspot, CursorMode.ForceSoftware); }
    public void SetCursorInteractable() { Cursor.SetCursor(cursor_interactable, cursor_hotspot, CursorMode.ForceSoftware); }
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
            ui_helper_base.ToggleActive(active);
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
    public void SetUIHelperGems(bool active, int[] gem_counts = null)
    {
        for (int i = 0; i < ui_helper_gems.Length; ++i)
        {
            if (active)
            {
                ui_helper_gems[i].enabled = true;
                ui_helper_gem_texts[i].enabled = true;
                ui_helper_gem_texts[i].text = gem_counts[i].ToString();
            }
            else
            {
                ui_helper_gems[i].enabled = false;
                ui_helper_gem_texts[i].enabled = false;
            }
        }
    }
    public void SetUIHelperText(string new_text)
    {
        ui_helper_general_text.text = new_text;
    }

    public void ChangeGemQuantity(int gem_tier, int change_amount)
    {
        if (gem_tier >= gem_quantities.Length)
        {
            Debug.Log("Invalid gem index");
            return;
        }

        gem_quantities[gem_tier] += change_amount;
        if (gem_quantities[gem_tier] >= max_gems)
        {
            gem_quantities[gem_tier] = max_gems;
            ui_gem_texts[gem_tier].color = maxed_color;
        }
        else
        {
            ui_gem_texts[gem_tier].color = original_gem_text_color;
        }

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
                ui_text_mining_level.color = maxed_color;
                ui_slider_mining_xp.value = 1f;
            }

            // Update UI
            SetMiningLvText();

            // Play effects
            particle_level_up.Play();
            Manager_Sounds.Instance.PlayLevelUp();
        }

        if (mining_level < max_mining_level)
        {
            ui_slider_mining_xp.value = (float)mining_xp / xp_needed;
        }
    }

    public void PressedSlot(int slot_num)
    {
        if (slot_extras[slot_num].GetTool().type == Tool_Type.None)
        {
            Manager_Sounds.Instance.PlayDenied();
            return;
        }

        Tool temp = slot_tool.GetTool();
        slot_tool.SetTool((slot_extras[slot_num].GetTool()));
        slot_extras[slot_num].SetTool(temp);
        ref_player.ClearTarget(); // Prevents tool changeups
        Manager_Sounds.Instance.PlayToolChange();
    }

    public void ClearSlot(int slot_num)
    {
        if (slot_extras[slot_num].GetTool().type == Tool_Type.None)
        {
            Manager_Sounds.Instance.PlayDenied();
            return;
        }

        slot_extras[slot_num].SetTool(new Tool());
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

        foreach (Tool_Map tm in tool_map)
        {
            TT_Sprite.Add(tm.tool, tm.sprite);
            TT_Name.Add(tm.tool, tm.name);
            TT_Description.Add(tm.tool, tm.description);
        }
    }

    private void Start()
    {
        // Set cursor
        SetCursorNormal();

        // Set helper
        ui_helper_last_id = 0;

        // Set all tools to their defaults
        slot_tool.SetTool(slot_tool.GetTool());
        slot_tool.SetTool(slot_tool.GetTool());
        foreach (Slot s in slot_extras)
        {
            s.SetTool(s.GetTool());
        }

        // Set gem colors in UI
        for (int i = 0; i < ui_gems.Length; ++i)
        {
            ui_gems[i].color = gem_colors[i];
            ui_helper_gems[i].color = gem_colors[i];
            ui_gem_texts[i].text = gem_quantities[i].ToString();
            original_gem_text_color = ui_gem_texts[i].color;
        }

        // Set mining level text
        mining_level = (mining_level > max_mining_level) ? max_mining_level : mining_level;
        SetMiningLvText();

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

    private void SetMiningLvText()
    {
        ui_text_mining_level.text = mining_level.ToString("D" + (Mathf.FloorToInt(Mathf.Log10(max_mining_level)) + 1));
    }
}
