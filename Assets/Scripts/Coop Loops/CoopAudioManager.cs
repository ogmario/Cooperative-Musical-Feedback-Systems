using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;


public class CoopAudioManager : MonoBehaviour
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

    bool finishedPlaying;
   
    public float[] shortTermAccelSD = new float[4];
    public float[] longTermAccelSDP1 = new float[4];
    public float[] longTermAccelSDP2= new float[4];
    public float[] longTermAccelSDP3 = new float[4];
    public float[] longTermAccelSDP4 = new float[4];

    public UpdateValues valuesP1;
    public UpdateValues valuesP2;
    public UpdateValues valuesP3;
    public UpdateValues valuesP4;

    public TextMeshProUGUI buttonText;
    public bool activePlayback;

    public AudioMixer audioMixer;
    public bool audioLevelMode;
    

    // Start is called before the first frame update
    void Start()
    {
        audioLevelMode = false;
        
        instrumentSources[0] = bassSource;
        instrumentSources[1] = pianoSource;
        instrumentSources[2] = melodySource;
        instrumentSources[3] = drumSource;              
    }

    // Update is called once per frame
    void Update()
    {      
        if (finishedPlaying&&activePlayback) // If the current loop has finished playing and music is active, update audio clips
        {
            finishedPlaying = false;
            longTermAccelSDP1 = valuesP1.accelAnalyzer.ObtainLongTermSD();
            longTermAccelSDP2 = valuesP2.accelAnalyzer.ObtainLongTermSD();
            longTermAccelSDP3 = valuesP3.accelAnalyzer.ObtainLongTermSD();
            longTermAccelSDP4 = valuesP4.accelAnalyzer.ObtainLongTermSD();

            bassSource.clip = bassClips[UpdateIndexes(longTermAccelSDP1[3])];
            pianoSource.clip = pianoClips[UpdateIndexes(longTermAccelSDP2[3])];
            melodySource.clip = melodyClips[UpdateIndexes(longTermAccelSDP3[3])];
            drumSource.clip = drumsClips[UpdateIndexes(longTermAccelSDP4[3])];

            foreach (AudioSource audioSource in instrumentSources)
            {
                audioSource.Play();
                audioSource.loop = true;
            }
          
            
            StartCoroutine(CheckClipEnd());
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
            valuesP1.accelAnalyzer.ClearLTLists();
            valuesP2.accelAnalyzer.ClearLTLists();
            valuesP3.accelAnalyzer.ClearLTLists();
            valuesP4.accelAnalyzer.ClearLTLists();
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
        bassSource.clip = bassClips[0];
        pianoSource.clip = pianoClips[0];
        melodySource.clip = melodyClips[0];
        drumSource.clip = drumsClips[0];
    }
    

}
