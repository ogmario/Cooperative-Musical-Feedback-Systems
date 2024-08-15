using UnityEngine;
using TMPro;
using System.Collections.Generic;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Receives accelerometer inputs and updates UI.
 *
 */
public class UpdateValues : MonoBehaviour
{
    public TextMeshProUGUI accelX;
    public TextMeshProUGUI accelY;
    public TextMeshProUGUI accelZ;

    public TextMeshProUGUI accelXFluctuationText;
    public TextMeshProUGUI accelYFluctuationText;
    public TextMeshProUGUI accelZFluctuationText;
    public TextMeshProUGUI totalAccelFluctuationText;
    
    public FluctuationAnalyzer accelAnalyzer;

    float[] shortTermAccelSD = new float[4];

    private AudioManager audioManager;
    private CoopAudioManager coopAudioManager;

    public bool singlePlayer;
    private Dictionary<string, int> tagToIndex = new Dictionary<string, int>();


    // Start is called before the first frame update
    void Start()
    {
       
       accelAnalyzer = new FluctuationAnalyzer(windowSize: 32);
       singlePlayer = this.tag == "UpdateValues";
       if(singlePlayer) //Check if this is a cooperative build or not
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
       else
        {
            coopAudioManager = FindObjectOfType<CoopAudioManager>();
            tagToIndex["P1"] = 0;
            tagToIndex["P2"] = 1;
            tagToIndex["P3"] = 2;
            tagToIndex["P4"] = 3;
        }
        
    }


    public void ObtainAccelValues(Vector3 values)
    {
        //Add incoming accelerometer input to lists
        accelAnalyzer.AddDataToLists(values);
        
        //Calculate short term SD
        shortTermAccelSD = accelAnalyzer.ObtainShortTermSD();

        // Send accel to audio manager
        if (singlePlayer)
        {
            SendAccel(shortTermAccelSD);
        }
        else
        {
            SendAccelCoop(shortTermAccelSD[3], tagToIndex[this.tag]);
        }        

        //Update UI Elements 
        accelXFluctuationText.text = "SD X: " +shortTermAccelSD[0];
        accelYFluctuationText.text = "SD Y: " + shortTermAccelSD[1];
        accelZFluctuationText.text = "SD Z: " + shortTermAccelSD[2];
        totalAccelFluctuationText.text = "Total SD: " + shortTermAccelSD[3];

        accelX.text = "X:" + values.x;
        accelY.text = "Y:" + values.y;
        accelZ.text = "Z:" + values.z;
        
    }

    //Send short term SD values to Audio Manager (one participant)
    public void SendAccel(float []accelValues)
    {
        audioManager.shortTermAccelSD = accelValues;        
    }
    //Send short term SD values to Cooperative Audio Manager (multiple participants)
    public void SendAccelCoop(float totalAccel, int index)
    {
        coopAudioManager.shortTermAccelSD[index] = totalAccel;
    }
}
