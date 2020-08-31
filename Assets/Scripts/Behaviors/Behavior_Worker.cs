using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Worker : Behavior_Seeker
{
    [SerializeField] private Behavior_Room script_room = null;

    [SerializeField] private Manager_Main.Tool tool = null;

    private bool activated = false;
    private bool reached_target = false;
    private float timeout_count = 0f;

    public void Activate()
    {
        activated = true;
    }

    public void SetStats(Manager_Main.Tool new_tool, float new_move_speed)
    {
        tool = new_tool;
        move_speed = new_move_speed;
    }

    public override Manager_Main.Tool GetTool()
    {
        return tool;
    }

    private void Update()
    {
        if (!activated)
        {
            return;
        }

        if (target == null)
        {
            Behavior_Spawner spawner = script_room.GetRandomSpawner();
            Behavior_Node node = spawner.GetActiveNode();
            if (node)
            {
                SetTarget(node);
                ref_ai_seeker.StartPath(transform.position, target.transform.position, OnPathReady);
                timeout_count = 0f;
                reached_target = false;
            }
        }
        else if (!reached_target)
        {
            if (ref_rbody_self.velocity.magnitude <= Parameters_Worker.Instance.stuck_velocity_threshold)
            {
                timeout_count += Time.deltaTime;

                if (timeout_count >= Parameters_Worker.Instance.stuck_timeout_seconds)
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
    }

    private void FixedUpdate()
    {
        if (!activated)
        {
            return;
        }

        Move();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!activated)
        {
            return;
        }

        if (target && collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && !target_activated)
        {
            TargetReached();
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
