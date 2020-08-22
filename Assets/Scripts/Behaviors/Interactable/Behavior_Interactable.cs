using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Behavior_Interactable : MonoBehaviour
{
    [SerializeField] private string tag_ui_helper = "";
    [SerializeField] protected string text_display = "";

    private GameObject ui_helper_base = null;
    private Text ui_helper_text = null;

    public abstract void Activate();
    public virtual void Deactivate() { }

    private void Awake()
    {
        ui_helper_base = GameObject.FindGameObjectWithTag(tag_ui_helper);
        ui_helper_text = ui_helper_base.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        ui_helper_base.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        Manager_Main.Instance.SetCursorInteractable();

        if (text_display != "")
        {
            ui_helper_base.gameObject.SetActive(true);
            ui_helper_text.text = text_display;

            /*
            float screen_width_half = Screen.width / 2f;
            float screen_height_half = Screen.height / 2f;
            float mouse_x = Input.mousePosition.x;
            float mouse_y = Input.mousePosition.y;
            float ui_rect_width_half = ui_text_helper.rectTransform.rect.width / 2f;
            float ui_rect_height_half = ui_text_helper.rectTransform.rect.height / 2f;

            // Place UI in location that would not block view
            Vector2 offset = Vector2.zero;
            if (mouse_x < screen_width_half && mouse_y < screen_height_half) // Bottom left region
            {
                offset = new Vector2(ui_rect_width_half, ui_rect_height_half);
                ui_text_helper.alignment = TextAnchor.LowerLeft;
            }
            else if (mouse_x < screen_width_half && mouse_y >= screen_height_half) // Top left region
            {
                offset = new Vector2(ui_rect_width_half, -ui_rect_height_half);
                ui_text_helper.alignment = TextAnchor.UpperLeft;
            }
            else if (mouse_x >= screen_width_half && mouse_y < screen_height_half) // Bottom right region
            {
                offset = new Vector2(-ui_rect_width_half, ui_rect_height_half);
                ui_text_helper.alignment = TextAnchor.LowerRight;
            }
            else if (mouse_x >= screen_width_half && mouse_y >= screen_height_half) // Top right region
            {
                offset = new Vector2(-ui_rect_width_half, -ui_rect_height_half);
                ui_text_helper.alignment = TextAnchor.UpperRight;
            }
            ui_text_helper.transform.position = (Vector2)Input.mousePosition + offset * ui_text_helper.canvas.scaleFactor;
            */
        }
    }

    private void OnMouseExit()
    {
        DisableHelperText();
    }

    protected void DisableHelperText()
    {
        Manager_Main.Instance.SetCursorNormal();
        if (ui_helper_base)
        {
            ui_helper_base.gameObject.SetActive(false);
        }
    }
}
