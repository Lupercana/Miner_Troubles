using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pathfinding;

public class Behavior_Player : MonoBehaviour
{
    [SerializeField] private string[] tag_interactables = null;
    [SerializeField] private Seeker ref_ai_seeker = null;
    [SerializeField] private Rigidbody2D ref_rbody_self = null;

    [SerializeField] private float move_speed = 0f;
    [SerializeField] private float waypoint_reached_threshold = 0f;

    private Path path_current = null;
    private Behavior_Interactable target = null;
    private int path_waypoint_current;
    private bool target_activated;

    private void Update()
    {
        // Don't respond to mouse over UI, gameobject refers to UI gameobject
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouse_world, Vector2.zero, 0f);
            string hit_tag = (hit) ? hit.transform.tag : "";
            foreach (string tag in tag_interactables)
            {
                if (tag == hit_tag)
                {
                    target = hit.transform.gameObject.GetComponent<Behavior_Interactable>();
                    target_activated = false;
                }
            }

            // Go towards clicked location even if not an interactable
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (target && collision.collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && !target_activated)
        {
            // We've reached the target, interact with it
            Debug.Log("A");
            target.Activate();
            target_activated = true;

            // Stop moving to prevent sliding off of target
            path_current = null;
            ref_rbody_self.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (target && collision.collider.gameObject.GetInstanceID() == target.gameObject.GetInstanceID() && target_activated)
        {
            Debug.Log("D");
            target.Deactivate();
            target = null;
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
