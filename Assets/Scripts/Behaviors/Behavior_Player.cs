using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Behavior_Player : MonoBehaviour
{
    [SerializeField] private Seeker ref_ai_seeker = null;
    [SerializeField] private Rigidbody2D ref_rbody_self = null;

    [SerializeField] private float move_speed = 0f;
    [SerializeField] private float waypoint_reached_threshold = 0f;

    private Path path_current = null;
    private int path_waypoint_current;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ref_ai_seeker.StartPath(transform.position, new Vector3(mouse_world.x, mouse_world.y, transform.position.z), OnPathReady);
        }
    }

    private void FixedUpdate()
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

    private void OnPathReady(Path p)
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
