using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Collections;
using GroqApiLibrary;
using NaughtyAttributes; // Ensure it's Unity-compatible

public class GroqVision : MonoBehaviour
{
    public RawImage rawImage; // Assign this in the Inspector
    [SerializeField] private string apiKey = "your_api_key_here";
    [SerializeField] private string model = "model_name";

    private GroqApiClient groqApi;

    void Start()
    {
        groqApi = new GroqApiClient(apiKey);
    }

    [Button]
    private void VisionRequest()
    {
        StartCoroutine(SendVisionRequest());
    }

    private IEnumerator SendVisionRequest()
    {
        Texture sourceTexture = rawImage.texture;

        if (sourceTexture == null)
        {
            Debug.LogError("RawImage has no texture.");
            yield break;
        }

        // Wait for at least one frame to ensure the webcam feed has data
        yield return new WaitForEndOfFrame();

        Texture2D texture2D;

        // If it's already a Texture2D, no conversion needed
        if (sourceTexture is Texture2D t2d)
        {
            texture2D = t2d;
        }
        // Convert from WebCamTexture to Texture2D
        else if (sourceTexture is WebCamTexture webcamTexture)
        {
            texture2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
            texture2D.SetPixels32(webcamTexture.GetPixels32());
            texture2D.Apply();
        }
        else
        {
            Debug.LogError("Unsupported texture type: " + sourceTexture.GetType());
            yield break;
        }

        byte[] imageBytes = texture2D.EncodeToJPG(); // or EncodeToPNG()
        string base64Image = Convert.ToBase64String(imageBytes);

        string prompt = "Describe this image";

        var task = groqApi.CreateVisionCompletionWithTempBase64ImageAsync(base64Image, prompt, model);

        while (!task.IsCompleted)
            yield return null;

        if (task.Exception != null)
        {
            Debug.LogError("Groq Vision API Error: " + task.Exception.Message);
        }
        else
        {
            var result = task.Result;
            string content = result?["choices"]?[0]?["message"]?["content"]?.ToString();
            Debug.Log("Groq describes: " + content);
        }
    }

}
