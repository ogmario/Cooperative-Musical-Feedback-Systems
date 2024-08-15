using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Changes music loops of each instrument depeding on the Long Term SD values. Changes mixer levels according to Short Term SD values.
 * Also in charge of playing and pausing music. *
 */

public class AudioManager : MonoBehaviour
{    
    public AudioSource bassSource;
    public AudioSource pianoSource;
    public AudioSource melodySource;
    public AudioSource drumSource;
    public AudioSource[] instrumentSources = new AudioSource[4];

    
    public AudioClip[] bassClips;
    public AudioClip[] pianoClips;
    public AudioClip[] melodyClips;
    public AudioClip[] drumsClips;


    private int bassIndex;
    private int pianoIndex;
    private int melodyIndex;
    private int drumIndex;


    bool finishedPlaying;

    public float[] shortTermGyroSD = new float[4];
    public float[] shortTermAccelSD = new float[4];
    public float[] longTermGyroSD = new float[4];
    public float[] longTermAccelSD = new float[4];

    private UpdateValues values;

    public TextMeshProUGUI buttonText;
    public bool activePlayback;

    public AudioMixer audioMixer;
    public bool audioLevelMode;

    public TextMeshProUGUI accelXFluctuationText;
    public TextMeshProUGUI accelYFluctuationText;
    public TextMeshProUGUI accelZFluctuationText;
    public TextMeshProUGUI totalAccelFluctuationText;


    // Start is called before the first frame update
    void Start()
    {
        audioLevelMode = false;
        
        instrumentSources[0] = bassSource;
        instrumentSources[1] = pianoSource;
        instrumentSources[2] = melodySource;
        instrumentSources[3] = drumSource;

        values = FindObjectOfType<UpdateValues>();        
    }

    // Update is called once per frame
    void Update()
    {
        // If the current loop has finished playing and music is active, update audio clips
        if (finishedPlaying&&activePlayback)
        {
            finishedPlaying = false;
            longTermAccelSD = values.accelAnalyzer.ObtainLongTermSD();          
                            
            bassSource.clip = bassClips[bassIndex];
            pianoSource.clip = pianoClips[pianoIndex];
            melodySource.clip = melodyClips[melodyIndex];
            drumSource.clip = drumsClips[drumIndex];

            foreach (AudioSource audioSource in instrumentSources)
            {
                audioSource.Play();
                audioSource.loop = true;
            }
          
            
            StartCoroutine(CheckClipEnd());
            StartCoroutine(UpdateIndexes());
        }
       
        // If dynamic audio levels are active, update mixer levels every frame according to short term SD
        if (audioLevelMode)
        {
            UpdateDBLevel("BassLevel", shortTermAccelSD[0]);
            UpdateDBLevel("PianoLevel", shortTermAccelSD[1]);
            UpdateDBLevel("MelodyLevel", shortTermAccelSD[2]);
            UpdateDBLevel("DrumsLevel", shortTermAccelSD[3]);
        }
    }

    // Obtain an index depending on the range of long term fluctuation
    public int UpdateIndexes(float fluctuation)
    {   
        int index;

        if (fluctuation < 1)
        {
            index = 0;
        }
        else if(fluctuation > 1 && fluctuation < 3)
        {
            index = 1;
        }
        else if (fluctuation > 3 && fluctuation < 5)
        {
            index = 2;
        }
        else
        {
            index = 3;
        }
        return index;        
    }
    
    // Update mixer level of a certain channel
    public void UpdateDBLevel(string channelName, float fluctuation)
    {
        audioMixer.SetFloat(channelName, LerpValue(fluctuation));
    }
    
    // Check if current audio clip has ended if an amount of time equal to the duration of the clip passes
    IEnumerator CheckClipEnd()
    {
        yield return new WaitForSeconds(8);
        finishedPlaying = true;
    }
    // Obtain an index depending on the range of long term fluctuation of each axis
    IEnumerator UpdateIndexes()
    {
        yield return new WaitForSeconds(7.5f);
        bassIndex = UpdateIndexes(longTermAccelSD[0]);
        pianoIndex = UpdateIndexes(longTermAccelSD[1]);
        melodyIndex = UpdateIndexes(longTermAccelSD[2]);
        drumIndex = UpdateIndexes(longTermAccelSD[3]);

        accelXFluctuationText.text = "SD X: " + longTermAccelSD[0];
        accelYFluctuationText.text = "SD Y: " + longTermAccelSD[1];
        accelZFluctuationText.text = "SD Z: " + longTermAccelSD[2];
        totalAccelFluctuationText.text = "Total SD: " + longTermAccelSD[3];
    }

    // Play/Stop the music
    public void ToggleAudio()
    {
        if (activePlayback)
        {
            foreach (var audioSource in instrumentSources)
            {
                audioSource.Stop();                
            }
            StopAllCoroutines();
            finishedPlaying = false;
            buttonText.text = "Play";
        }
        else
        {
            ResetDensityIndexes();
            foreach (var audioSource in instrumentSources)
            {
                audioSource.Play();                
            }
            StartCoroutine(CheckClipEnd());
            StartCoroutine(UpdateIndexes());
            values.accelAnalyzer.ClearLTLists();
            finishedPlaying = false;
            buttonText.text = "Stop";            
        }

        activePlayback = !activePlayback;
    }
    // Activate dynamic mixer levels
    public void ActivateDynamicAudioLevel()
    {
        audioLevelMode = !audioLevelMode;
        ResetMixerLevels();
    }

    // Adjust mixer levels between 0 dB and -6 dB using linear interpolation, depeding on the incoming value
    private float LerpValue(float value)
    {
        if (value <= 0f) return -6f;
        if (value >= 10f) return 0f;

        float mappedValue = Mathf.Lerp(-6f, 0f, value / 9f);

        return mappedValue;
    }
    // Set all mixer channels back to 0 dB
    private void ResetMixerLevels()
    {
        audioMixer.SetFloat("BassLevel", 0);
        audioMixer.SetFloat("PianoLevel", 0);
        audioMixer.SetFloat("MelodyLevel", 0);
        audioMixer.SetFloat("DrumsLevel", 0);
    }

    // Set all audio clips to the lowest intensity
    private void ResetDensityIndexes()
    {
        bassIndex = 0;
        pianoIndex = 0;
        melodyIndex = 0;
        drumIndex = 0;

        bassSource.clip = bassClips[0];
        pianoSource.clip = pianoClips[0];
        melodySource.clip = melodyClips[0];
        drumSource.clip = drumsClips[0];
    }
    

}
