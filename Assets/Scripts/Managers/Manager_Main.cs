using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Main : MonoBehaviour
{
    [SerializeField] private Image[] ui_gems = null;

    [SerializeField] private Color[] gem_colors = null;

    private void Start()
    {
        // Set gem colors in UI
        for (int i = 0; i < ui_gems.Length; ++i)
        {
            ui_gems[i].color = gem_colors[i];
        }
    }
}
