using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Singleton : MonoBehaviour
{
    private static Parameters_Singleton Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
