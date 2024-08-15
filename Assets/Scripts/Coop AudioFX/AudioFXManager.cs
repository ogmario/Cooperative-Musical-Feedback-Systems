using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Updates Audio Fx according to number of movements whenever a looping timer ends. Also plays and stops music.
*/
public class AudioFXManager : MonoBehaviour
{
    public AudioSource songSource;
    public AudioMixer mixer;
    bool activePlayback;   
       
    public float maxReps = 8;
    public float minReps = 0;

    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI distText;
    public TextMeshProUGUI cutoffText;
    
    public Counter counterP1;
    public Counter counterP2;

    public FilePicker filePicker;
    public string oldFilePath;

    float maxExponent;
    // Start is called before the first frame update

    void Start()
    {
        maxExponent = Mathf.Log10(22000);
        activePlayback = false;
        oldFilePath = null;
    }
    // Play and stop music
    public void ToggleAudio()
    {
        if (activePlayback)
        {
            StopMusic();  
        }
        else
        {
            PlayMusic(); 
        }

        activePlayback = !activePlayback;
    }
    //Stop music
    public void StopMusic()
    {
        songSource.Stop();
        buttonText.text = "Play";
        filePicker.path.text = "";
        counterP1.PauseCounter();
        counterP2.PauseCounter();
        StopAllCoroutines();
    }
    //Prepare to play music
    public void PlayMusic()
    {
        if(oldFilePath!=filePicker.filePath)
        {
            filePicker.PlayAudioFromFile();
            oldFilePath = filePicker.filePath;
        }

        Invoke("ActuallyPlay", 0.5f);
        counterP1.ActivateCounter();
        counterP2.ActivateCounter();
        StartCoroutine(IncrementTracker());
        buttonText.text = "Stop";
        filePicker.path.text = "Now Playing: " + filePicker.filename;
    }
    //Play music
    public void ActuallyPlay()
    {
        songSource.Play();
    }
    // Looping timer, updates effects whenever it ends
    IEnumerator IncrementTracker()
    {
        float timer=0;

        while (true)
        {
            // Check if the timer exceeds 8 seconds
            if (timer >= 4f)
            {
                // Update increments in last 8 seconds
                UpdateFX(counterP1.counter, counterP2.counter);

                // Reset count and timer
                counterP1.ResetCounter();
                counterP2.ResetCounter();
                timer = 0f;
            }

            // Increment timer
            timer += Time.deltaTime;

            yield return null;
        }
    }

    // Calculate FX parameters and update UI
    public void UpdateFX(int valueP1, int valueP2)
    {
        float dist = DistNormalization(valueP1);
        float cutoff = FilterNormalization(valueP2);

        mixer.SetFloat("Dist", dist);
        mixer.SetFloat("Cutoff", cutoff);

        distText.text = "Distortion: " + dist;
        cutoffText.text = "Cutoff Frequency: " + cutoff;
    }
    //Map distortion levels from 0 to 0.5 depending on counter
    public float DistNormalization(int value)
    {
        float distLevel;
        if (value >= maxReps)
        {
            distLevel = 0;
        }
        else
        {
            distLevel = 0.5f*(1 - (value / maxReps));
        }
        
        return distLevel;
    }
    //Map filter cutoff logarithmically from 100 to 22000 depending on counter
    float FilterNormalization(int input)
    {
        input = Mathf.Clamp(input, 0, 8);
        float exponent = Mathf.Lerp(2, maxExponent, input / maxReps);
        float output = Mathf.Clamp(Mathf.Pow(10, exponent), 100, 22000);

        return output;
    }
}
