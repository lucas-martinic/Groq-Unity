using UnityEngine;
using System.Collections;
using System.Text.Json.Nodes;
using GroqApiLibrary; // Ensure this is compatible with Unity and included in your Assets

public class GroqChat : MonoBehaviour
{
    [SerializeField] private string apiKey = "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";

    private GroqApiClient groqApi;

    void Start()
    {
        groqApi = new GroqApiClient(apiKey);
        StartCoroutine(SendChatRequest());
    }

    private IEnumerator SendChatRequest()
    {
        var request = new JsonObject
        {
            ["model"] = "llama-3.1-8b-instant",
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = "Hello, Groq! What can you do?"
                }
            }
        };

        // Call async method and yield until it's done
        var task = groqApi.CreateChatCompletionAsync(request);

        while (!task.IsCompleted)
            yield return null;

        if (task.Exception != null)
        {
            Debug.LogError("Error: " + task.Exception.Message);
        }
        else
        {
            var result = task.Result;
            string content = result?["choices"]?[0]?["message"]?["content"]?.ToString();
            Debug.Log("Groq says: " + content);
        }
    }
}
