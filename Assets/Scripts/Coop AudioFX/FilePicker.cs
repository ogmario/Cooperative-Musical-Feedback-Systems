using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Loads an audio file using NativeFilePicker
*
*/

public class FilePicker : MonoBehaviour
{
    public string filePath;
    public AudioSource audioSource;
    public TextMeshProUGUI path;
    public string filename;

    private void Start()
    {
        filePath = null;
    }
    // Use Native File Picker to obtain a file path
    public void LoadFile()
    {
        string[] FileType = new string[] { "audio/*" };

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                filePath = path;
                Debug.Log("Picked file: " + filePath);
            }
            
        }, FileType);
    }
    // Once a path has been obtained, load audio
    public void PlayAudioFromFile()
    {
        if (filePath != null)
        {
            filename = Path.GetFileName(filePath);
            StartCoroutine(LoadAudioClipFromFile());
        }
        
    }

    // Coroutine to load an audio clip from a file
    private IEnumerator LoadAudioClipFromFile()
    {
        string fullFilePath = "file://" + filePath;

        // Load the audio file using WWW
        using (WWW www = new WWW(fullFilePath))
        {
            yield return www;

            // Check for errors
            if (www.error != null)
            {
                path.text = "Error: " + www.error;
                Debug.LogError("Error loading audio file: " + www.error);
                yield break;
            }

            // Assign the loaded audio clip to the audio source
            audioSource.clip = www.GetAudioClip(false, false);                       
        }
    }
}



