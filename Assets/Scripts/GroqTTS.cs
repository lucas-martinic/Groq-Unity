using NaughtyAttributes;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum PlayAIVoice
{
    Aaliyah_PlayAI,
    Adelaide_PlayAI,
    Angelo_PlayAI,
    Arista_PlayAI,
    Atlas_PlayAI,
    Basil_PlayAI,
    Briggs_PlayAI,
    Calum_PlayAI,
    Celeste_PlayAI,
    Cheyenne_PlayAI,
    Chip_PlayAI,
    Cillian_PlayAI,
    Deedee_PlayAI,
    Eleanor_PlayAI,
    Fritz_PlayAI,
    Gail_PlayAI,
    Indigo_PlayAI,
    Jennifer_PlayAI,
    Judy_PlayAI,
    Mamaw_PlayAI,
    Mason_PlayAI,
    Mikail_PlayAI,
    Mitch_PlayAI,
    Nia_PlayAI,
    Quinn_PlayAI,
    Ruby_PlayAI,
    Thunder_PlayAI
}

public class GroqTTS : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private const string apiUrl = "https://api.groq.com/openai/v1/audio/speech";
    [SerializeField] private string apiKey = "your_groq_api_key_here";
    private const string model = "playai-tts";
    [SerializeField] private PlayAIVoice selectedVoice = PlayAIVoice.Fritz_PlayAI;
    private const string responseFormat = "wav";
    [SerializeField] private string prompt = "I love building and shipping new features for our students!";

    [Button]
    private async void Generate()
    {
        await GenerateAndPlaySpeech(prompt);
    }

    public async Task GenerateAndPlaySpeech(string text)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var json = $"{{\"model\":\"{model}\",\"voice\":\"{GetVoiceName(selectedVoice)}\",\"input\":\"{text}\",\"response_format\":\"{responseFormat}\"}}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"TTS API call failed: {response.StatusCode}");
                Debug.LogError(await response.Content.ReadAsStringAsync());
                return;
            }

            byte[] audioData = await response.Content.ReadAsByteArrayAsync();
            AudioClip clip = CreateClipFromWav(audioData);
            audioSource.clip = clip;
            audioSource.Play();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error generating TTS: " + ex.Message);
        }
    }

    private string GetVoiceName(PlayAIVoice voice)
    {
        return voice.ToString().Replace('_', '-');
    }

    private AudioClip CreateClipFromWav(byte[] wav)
    {
        int channels = BitConverter.ToInt16(wav, 22);
        int sampleRate = BitConverter.ToInt32(wav, 24);
        int bitsPerSample = BitConverter.ToInt16(wav, 34);

        int dataStart = FindDataChunk(wav) + 8;
        int sampleCount = (wav.Length - dataStart) / (bitsPerSample / 8);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            int sampleIndex = dataStart + i * 2; // assuming 16-bit PCM
            short sample = BitConverter.ToInt16(wav, sampleIndex);
            samples[i] = sample / 32768f;
        }

        AudioClip clip = AudioClip.Create("GroqTTS_Audio", sampleCount / channels, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private int FindDataChunk(byte[] wav)
    {
        for (int i = 12; i < wav.Length - 4; i++)
        {
            if (wav[i] == 'd' && wav[i + 1] == 'a' && wav[i + 2] == 't' && wav[i + 3] == 'a')
            {
                return i;
            }
        }
        throw new Exception("DATA chunk not found in WAV");
    }
}
