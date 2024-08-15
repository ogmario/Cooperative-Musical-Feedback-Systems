using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Calculates short term and long term standard deviation.
 */
public class FluctuationAnalyzer
{
    private List<float> shortTermX;
    private List<float> shortTermY;
    private List<float> shortTermZ;
    private List<float> shortTermTotal;
    private int windowSize;
    float[] shortTermSD = new float[4];

    private List<float> longTermX;
    private List<float> longTermY;
    private List<float> longTermZ;
    private List<float> longTermTotal;
    private float[] longTermSD = new float[4];

    public FluctuationAnalyzer(int windowSize)
    {
        this.windowSize = windowSize;
        shortTermX = new List<float>(windowSize);
        shortTermY = new List<float>(windowSize);
        shortTermZ = new List<float>(windowSize);
        shortTermTotal = new List<float>(windowSize);

        longTermX = new List<float>();
        longTermY = new List<float>();
        longTermZ = new List<float>();
        longTermTotal = new List<float>();
    }
    // Add incoming accelerometer values to lists (called from UpdateValues.cs)
    public void AddDataToLists(Vector3 vector)
    {
        //Short Term Lists
        shortTermTotal.Add(vector.x);
        shortTermTotal.Add(vector.y);
        shortTermTotal.Add(vector.z);

        shortTermX.Add(vector.x);
        shortTermY.Add(vector.y);
        shortTermZ.Add(vector.z);

        //Check Window Size
        CheckListSize(shortTermTotal);
        CheckListSize(shortTermX);
        CheckListSize(shortTermY);
        CheckListSize(shortTermZ);

        //LongTerm Lists
        longTermX.Add(vector.x);
        longTermY.Add(vector.y);
        longTermZ.Add(vector.z);

        longTermTotal.Add(vector.x);
        longTermTotal.Add(vector.y);
        longTermTotal.Add(vector.z);
    }
    // Sliding window approach: if a Short Term List exceeds window size, remove the first value
    private void CheckListSize(List<float> dataList)
    {
        if (dataList.Count > windowSize)
        {
            dataList.RemoveAt(0);
        }
    }
    // Calculates standard deviation of a list of values
    private float CalculateSD(List<float> dataList)
    {
        try
        {
            if (dataList == null || dataList.Count == 0)
            {
                ///Debug.LogWarning("Data list is null or empty. Returning 0.");
                return 0f;
            }

            float average = dataList.Average();
            float standardDeviation = Mathf.Sqrt(dataList.Sum(d => Mathf.Pow(d - average, 2)) / dataList.Count);

            return standardDeviation;
        }
        catch (Exception e)
        {
            Debug.LogError("Unexpected error: " + e.Message);
            return float.NaN; // Return NaN (Not a Number) to indicate an error
        }
    }

    // Clear data from Long Term Lists
    public void ClearLTLists()
    {
        longTermX.Clear();
        longTermY.Clear();
        longTermZ.Clear();
        longTermTotal.Clear();
    }
    // Calculate Short Term Standard Deviation
    public float[] ObtainShortTermSD()
    {
        shortTermSD[0] = CalculateSD(shortTermX);
        shortTermSD[1] = CalculateSD(shortTermY);
        shortTermSD[2] = CalculateSD(shortTermZ);
        shortTermSD[3] = (shortTermSD[0] + shortTermSD[1] + shortTermSD[2]) / 3;

        return shortTermSD;
    }
    // Calculate Short Term Standard Deviation, then clear lists
    public float[] ObtainLongTermSD()
    {
        longTermSD[0] = CalculateSD(longTermX);
        longTermSD[1] = CalculateSD(longTermY);
        longTermSD[2] = CalculateSD(longTermZ);
        longTermSD[3] = (longTermSD[0] + longTermSD[1] + longTermSD[2]) / 3;

        ClearLTLists();

        return longTermSD;
    }


}


