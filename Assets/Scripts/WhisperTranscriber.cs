using NaughtyAttributes;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

public class WhisperTranscriber : MonoBehaviour
{
    public string whisperEndpoint = "https://api.groq.com/openai/v1/audio/transcriptions";
    public string apiKey = "YOUR_API_KEY_HERE";
    public MicRecorder recorder;

    [Button]
    private void TranscribeLatestAudio()
    {
        StartCoroutine(Transcribe(recorder.GetLastFilePath(), OnTranscription));
    }

    public IEnumerator Transcribe(string filePath, System.Action<string> onResult)
    {
        byte[] audioData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddField("model", "whisper-large-v3");
        form.AddField("language", "en");
        form.AddField("prompt", "Transcribe the following audio");
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");

        UnityWebRequest request = UnityWebRequest.Post(whisperEndpoint, form);
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Transcription failed: " + request.error);
            onResult?.Invoke(null);
        }
        else
        {
            string json = request.downloadHandler.text;
            string transcript = ParseTranscription(json);
            onResult?.Invoke(transcript);
        }
    }

    string ParseTranscription(string json)
    {
        int start = json.IndexOf("\"text\":\"") + 8;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    void OnTranscription(string text)
    {
        Debug.Log("Transcription:" + text);
    }
}
