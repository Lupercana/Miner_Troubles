using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Behavior_Player : MonoBehaviour
{
    [SerializeField] private Seeker ref_ai_seeker = null;
    [SerializeField] private Rigidbody2D ref_rbody_self = null;

    private Path path_current = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            path_current = ref_ai_seeker.StartPath(transform.position, new Vector3(mouse_world.x, mouse_world.y, transform.position.z), OnPathComplete);
        }
    }

    private void FixedUpdate()
    {
        if (path_current == null)
        {
            return;
        }


    }

    private void OnPathComplete(Path p)
    {
        Debug.Log("Completed path");
    }
}
