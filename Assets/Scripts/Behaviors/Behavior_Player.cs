using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Behavior_Player : Behavior_Seeker
{
    public override Manager_Main.Tool GetTool()
    {
        return Manager_Main.Instance.slot_tool.GetTool();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
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
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (target && target.gameObject.activeInHierarchy && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && !target_activated)
        {
            // We've reached the target, interact with it
            target.Activate(this);
            target_activated = true;

            // Stop moving to prevent sliding off of target
            path_current = null;
            ref_rbody_self.velocity = Vector2.zero;
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
