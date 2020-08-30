using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Delete_Menu : MonoBehaviour
{
    [SerializeField] private RectTransform ref_self_rect_transform = null;
    [SerializeField] private Image[] ref_slot_images = null;
    [SerializeField] private Image[] ref_mirror_images = null;

    public void RefreshGraphics()
    {
        for (int i = 0; i < ref_mirror_images.Length; ++i)
        {
            ref_mirror_images[i].sprite = ref_slot_images[i].sprite;
            ref_mirror_images[i].color = ref_slot_images[i].color;
        }
    }

    private void Update()
    {   
        if (Input.anyKeyDown) // Includes mouse
        {
            bool deactivate = true;
            if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(ref_self_rect_transform, Input.mousePosition)) // Left mouse
            {
                deactivate = false;
            }

            if (Input.GetButtonDown("Slot_1"))
            {
                Manager_Main.Instance.ClearSlot(0);
                deactivate = false;
            }
            if (Input.GetButtonDown("Slot_2"))
            {
                Manager_Main.Instance.ClearSlot(1);
                deactivate = false;
            }
            if (Input.GetButtonDown("Slot_3"))
            {
                Manager_Main.Instance.ClearSlot(2);
                deactivate = false;
            }
            if (Input.GetButtonDown("Slot_4"))
            {
                Manager_Main.Instance.ClearSlot(3);
                deactivate = false;
            }

            if (deactivate)
            {
                gameObject.SetActive(false);
            }
            else
            {
                RefreshGraphics();
            }
        }
    }

    private void OnEnable()
    {
        RefreshGraphics();
    }
}
