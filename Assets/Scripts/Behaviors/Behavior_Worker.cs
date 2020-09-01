using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Worker : Behavior_Seeker
{
    [SerializeField] private Behavior_Room script_room = null;

    [SerializeField] private SpriteRenderer ref_self_sprite_renderer = null;

    [SerializeField] private Manager_Main.Tool tool = null;

    private bool activated = false;
    private bool reached_target = false;
    private float timeout_count = 0f;
    private float last_activate_time = 0f;
    private float activate_delay_seconds = 1f;

    public void Activate()
    {
        activated = true;
        ref_self_rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override Manager_Main.Tool GetTool()
    {
        return tool;
    }

    public void SetTool(Manager_Main.Tool new_tool)
    {
        tool = new_tool;
        move_speed = Parameters_Interactables.Instance.worker_move_speeds[tool.tier];
        activate_delay_seconds = Parameters_Interactables.Instance.worker_activate_delay_seconds[tool.tier];
        ref_self_sprite_renderer.color = Manager_Main.Instance.GetGemColors()[tool.tier];
    }

    private void Start()
    {
        SetTool(tool);
    }

    private void Update()
    {
        if (!activated)
        {
            return;
        }

        if (target == null || !target.gameObject.activeInHierarchy) // Don't have target or target is no longer active
        {
            Behavior_Spawner spawner = script_room.GetRandomSpawner();
            Behavior_Node node = spawner.GetActiveNode();
            if (node)
            {
                SetTarget(node);
                ref_ai_seeker.StartPath(transform.position, target.transform.position, OnPathReady);
                timeout_count = 0f;
                last_activate_time = 0f;
                reached_target = false;
            }
        }
        else if (!reached_target) // Have valid target but have not reached it yet
        {
            if (ref_self_rbody.velocity.magnitude <= Parameters_Interactables.Instance.worker_stuck_velocity_threshold)
            {
                timeout_count += Time.deltaTime;

                if (timeout_count >= Parameters_Interactables.Instance.worker_stuck_timeout_seconds)
                {
                    ref_ai_seeker.StartPath(transform.position, target.transform.position, OnPathReady);
                    timeout_count = 0f;
                }
            }
            else
            {
                timeout_count = 0f;
            }
        }
        else // Reached target
        {
            float e_time = Time.time - last_activate_time;
            if (e_time >= activate_delay_seconds)
            {
                target.Activate(this);
                last_activate_time = Time.time;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!activated)
        {
            return;
        }

        Move();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!activated)
        {
            return;
        }

        if (target && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && !target_activated)
        {
            // Stop moving to prevent sliding off of target
            path_current = null;
            ref_self_rbody.velocity = Vector2.zero;
            target_activated = true;
            reached_target = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!activated)
        {
            return;
        }

        if (target && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && target_activated)
        {
            ClearTarget();
        }
    }
}
