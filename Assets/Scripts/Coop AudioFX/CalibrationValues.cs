using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Stores movement counter calibration values. This gameObject will persist across scenes.
*/
public class CalibrationValues : MonoBehaviour
{
    public static CalibrationValues Instance;
    public float delayThreshold = 0.2f;
    public float movementThreshold = 0.2f;   

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance because another one already exists
            return;
        }
        Instance = this; // Set this as the singleton instance

        // Make the GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);
    }

}
