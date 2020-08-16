using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Trans_Axis : MonoBehaviour
{
    enum Axis
    {
        x,
        y,
        z
    };

    [SerializeField] private Camera ref_camera = null;

    [SerializeField] private Axis transparency_axis = Axis.z;

    private void Start()
    {
        ref_camera.transparencySortMode = TransparencySortMode.CustomAxis;
        switch (transparency_axis)
        {
            case Axis.x:
                ref_camera.transparencySortAxis = new Vector3(1f, 0f, 0f);
                break;
            case Axis.y:
                ref_camera.transparencySortAxis = new Vector3(0f, 1f, 0f);
                break;
            default:
                ref_camera.transparencySortAxis = new Vector3(0f, 0f, 1f);
                break;
        }
    }
}
