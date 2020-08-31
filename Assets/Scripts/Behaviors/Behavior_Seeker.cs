using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Behavior_Seeker : MonoBehaviour
{
    [SerializeField] protected string[] tag_interactables = null;
    [SerializeField] protected Seeker ref_ai_seeker = null;
    [SerializeField] protected Rigidbody2D ref_rbody_self = null;

    [SerializeField] protected float move_speed = 0f;
    [SerializeField] protected float waypoint_reached_threshold = 0f;

    protected Path path_current = null;
    protected Behavior_Interactable target = null;
    protected int path_waypoint_current;
    protected bool target_activated = false;

    public abstract Manager_Main.Tool GetTool();

    public virtual void ClearTarget()
    {
        if (target)
        {
            target.Deactivate();
            target = null;
        }
    }

    protected void DeactivatePreviousTarget()
    {
        if (target)
        {
            target.Deactivate();
            target = null;
        }
    }

    protected void SetTarget(Behavior_Interactable new_target)
    {
        target = new_target;
        target_activated = false;
    }

    protected void Move() // Call in FixedUpdate()
    {
        if (path_current == null)
        {
            return;
        }

        if (path_waypoint_current < path_current.vectorPath.Count)
        {
            // Have not completed path yet
            Vector2 target_waypoint = path_current.vectorPath[path_waypoint_current];

            Vector2 move_dir = (target_waypoint - (Vector2)transform.position).normalized;
            Vector2 move_force = move_dir * move_speed * Time.fixedDeltaTime;
            ref_rbody_self.AddForce(move_force);

            if (Vector2.Distance(transform.position, target_waypoint) < waypoint_reached_threshold)
            {
                ++path_waypoint_current;
            }
        }
        else
        {
            // Completed path
            path_current = null;
        }
    }

    protected void TargetReached()
    {
        // We've reached the target, interact with it
        target.Activate(this);
        target_activated = true;

        // Stop moving to prevent sliding off of target
        path_current = null;
        ref_rbody_self.velocity = Vector2.zero;
    }

    protected void OnPathReady(Path p)
    {
        if (!p.error)
        {
            path_current = p;
            path_waypoint_current = 0;
        }
        else
        {
            Debug.Log("Path generation error");
        }
    }
}
