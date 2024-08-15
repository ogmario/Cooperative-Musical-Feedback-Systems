using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*
 * Author:Mario Ortega García
 * Date:August 2024
 * Description: movement counter which tracks significant movements considering accelerometer readings
 */
public class Counter : MonoBehaviour
{
    private Vector3 lastFrameAcc;
    private bool hasSignChanged = false;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI totalCounterText;
    public int counter;
    public int totalCounter;

    public TextMeshProUGUI xAccel;
    public TextMeshProUGUI yAccel;
    public TextMeshProUGUI zAccel;

    private float timeSinceLastIncrement = 0f;
    public  float delayThreshold = 0.2f;
    public float movementThreshold = 0.2f;  // Threshold for movement detection
    public Slider accelSlider;
    public Slider delaySlider;
    public TextMeshProUGUI accelSliderText;
    public TextMeshProUGUI delaySliderText;

    private CalibrationValues calibValues;
    public bool countReps = false;
    
    

    void Start()
    {
        lastFrameAcc = Input.acceleration;
        calibValues = FindObjectOfType<CalibrationValues>(); 
    }

    void Update()
    {
        if(countReps)
        {
            timeSinceLastIncrement += Time.deltaTime; //timer to measure time since last movement
        }
       
    }

    public void UpdateAccelValues(Vector3 accel)
    {
        Debug.Log("Called");
        if (countReps) //If counter active
        {
            Debug.Log("Called and passed");
            Vector3 currentAcc = accel; // get current accel values


            // Check for significant sign change on any axis
            if ((SignificantSignChange(lastFrameAcc.x, currentAcc.x) ||
                SignificantSignChange(lastFrameAcc.y, currentAcc.y) ||
                SignificantSignChange(lastFrameAcc.z, currentAcc.z)) && timeSinceLastIncrement >= delayThreshold)
            {
                if (!hasSignChanged)
                {
                    Debug.Log("Signed changed");
                    hasSignChanged = true; // Significant movement
                    OnSignChange(); //Increment counter
                    timeSinceLastIncrement = 0f; //Reset delay timer
                }
            }
            else
            {
                hasSignChanged = false; // No significant movement
            }

            lastFrameAcc = currentAcc;
            UpdateUI(currentAcc); //Update UI
            
        }
    }

    private bool SignificantSignChange(float lastValue, float currentValue)
    {
        // Check both the sign change and if the change exceeds the threshold
        return ((lastValue >= 0 && currentValue < 0) || (lastValue < 0 && currentValue >= 0)) &&
               Mathf.Abs(currentValue - lastValue) > movementThreshold;
    }
    //Increase counter value
    private void OnSignChange()
    {
        counter++;
        counterText.text = "Short Term Counter: " + counter;
        totalCounter++;
        totalCounterText.text = "Total Counter: " + totalCounter.ToString();
    }
    //Reset this counter
    public void ResetCounter()
    {
        counter = 0;
        counterText.text = "Short Term Counter: " + counter;
        timeSinceLastIncrement = 0f;  // Reset the timer on counter reset
    }
    //Reset all counters
    public void ResetBothCounters()
    {
        counter = 0;
        counterText.text = "Short Term Counter: " + counter;
        totalCounter = 0;
        totalCounterText.text = "Total Counter: " + totalCounter;

        timeSinceLastIncrement = 0f;  // Reset the timer on counter reset
    }
    //Continue counting
    public void ActivateCounter()
    {
        countReps = true;
    }
    //Stop Counting
    public void PauseCounter()
    {
        countReps = false;
    }

    //Update UI accelerometer values
    public void UpdateUI(Vector3 vector)
    {
        xAccel.text = "X: " + vector.x;
        yAccel.text = "Y: " + vector.y;
        zAccel.text = "Z: " + vector.z;
    }

    //Set minumum acceleration difference threshold with the slider
    public void SetAccelThreshold()
    {
        movementThreshold = accelSlider.value;
        accelSliderText.text = accelSlider.value.ToString("F2");
    }
    //Set delay threshold with the slider
    public void SetDelayThreshold()
    {
        delayThreshold = delaySlider.value;
        delaySliderText.text = delaySlider.value.ToString("F2");
    }
}



