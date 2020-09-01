using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Behavior_Player : Behavior_Seeker
{
    [SerializeField] private CircleCollider2D ref_self_interact_trigger = null;

    private bool using_mouse = false;
    private float move_x;
    private float move_y;

    public override Manager_Main.Tool GetTool()
    {
        return Manager_Main.Instance.slot_tool.GetTool();
    }

    private void Update()
    {
        // Mouse Controls
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            using_mouse = true;
            DeactivatePreviousTarget();

            Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouse_world, Vector2.zero, 0f);
            foreach (RaycastHit2D hit in hits)
            {
                string hit_tag = hit.transform.tag;
                foreach (string tag in tag_interactables)
                {
                    if (tag == hit_tag)
                    {
                        SetTarget(hit.transform.gameObject.GetComponent<Behavior_Interactable>());
                    }
                }
            }

            // Go towards clicked location even if not an interactable
            ref_ai_seeker.StartPath(transform.position, new Vector3(mouse_world.x, mouse_world.y, transform.position.z), OnPathReady);
        }

        // Keyboard Controls
        move_x = Input.GetAxis("Horizontal");
        move_y = Input.GetAxis("Vertical");
        bool pressed_interact = Input.GetButtonDown("Interact");
        if (Input.GetButtonDown("Slot_1")) { Manager_Main.Instance.PressedSlot(0); }
        if (Input.GetButtonDown("Slot_2")) { Manager_Main.Instance.PressedSlot(1); }
        if (Input.GetButtonDown("Slot_3")) { Manager_Main.Instance.PressedSlot(2); }
        if (Input.GetButtonDown("Slot_4")) { Manager_Main.Instance.PressedSlot(3); }
        if (Input.GetButtonDown("Slot_4")) { Manager_Main.Instance.PressedSlot(3); }
        if (Input.GetButtonDown("Slot_Delete")) { Manager_Main.Instance.PressedSlotDelete(); }

        if ((move_x != 0f || move_y != 0f || pressed_interact) && using_mouse) // Inputs comes with deadzones
        {
            using_mouse = false;
            path_current = null;
            ref_self_rbody.velocity = Vector2.zero; // Eliminate any previous velocity from to pathfinding
        }

        if (pressed_interact)
        {
            target = null;
            RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position + ref_self_interact_trigger.offset, ref_self_interact_trigger.radius, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                string hit_tag = hit.transform.tag;
                foreach (string tag in tag_interactables)
                {
                    if (tag == hit_tag)
                    {
                        target = hit.transform.gameObject.GetComponent<Behavior_Interactable>();
                    }
                }
            }
            if (target)
            {
                target.Activate(this);
                target_activated = true;

                ref_self_rbody.velocity = Vector2.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        if (using_mouse)
        {
            Move();
        }
        else
        {
            Vector2 force_x = move_x * move_speed * Time.fixedDeltaTime * Vector2.right;
            Vector2 force_y = move_y * move_speed * Time.fixedDeltaTime * Vector2.up;
            ref_self_rbody.AddForce(force_x + force_y);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (using_mouse && target && target.gameObject.activeInHierarchy && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && !target_activated)
        {
            // We've reached the target, interact with it
            target.Activate(this);
            target_activated = true;

            // Stop moving to prevent sliding off of target
            path_current = null;
            ref_self_rbody.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (target && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && target_activated)
        {
            ClearTarget();
        }
    }
}
