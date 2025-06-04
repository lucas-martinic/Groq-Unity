using UnityEngine;
using System.IO;
using NaughtyAttributes;

public class MicRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private string micDevice;
    private string filePath;

    public int duration = 10; // seconds
    public int sampleRate = 44100;

    void Start()
    {
        micDevice = Microphone.devices[0];
    }

    [Button]
    public void StartRecording()
    {
        recordedClip = Microphone.Start(micDevice, false, duration, sampleRate);
        Debug.Log("Recording...");
    }

    [Button]
    public void StopAndSave()
    {
        Microphone.End(micDevice);
        Debug.Log("Recording stopped.");

        SaveWav("recorded_audio", recordedClip);
    }

    void SaveWav(string filename, AudioClip clip)
    {
        filePath = Path.Combine(Application.persistentDataPath, filename + ".wav");
        if(SavWav.Save(filename, clip))
        {
            Debug.Log("Saved WAV to: " + filePath);
        }
    }

    public string GetLastFilePath() => filePath;
}
