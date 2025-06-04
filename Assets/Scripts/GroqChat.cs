using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using GroqApiLibrary;
using NaughtyAttributes;

public class GroqChat : MonoBehaviour
{
    [SerializeField] private string apiKey;
    [SerializeField, TextArea] private string userInput = "Hello, Groq! What can you do?";

    private GroqApiClient groqApi;
    private List<JsonObject> messageHistory = new List<JsonObject>();

    [Button]
    void SendRequest()
    {
        if (groqApi == null)
            groqApi = new GroqApiClient(apiKey);

        // Add new user message to history
        messageHistory.Add(new JsonObject
        {
            ["role"] = "user",
            ["content"] = userInput
        });

        StartCoroutine(SendChatRequest());
    }

    private IEnumerator SendChatRequest()
    {
        var messagesArray = new JsonArray();

        foreach (var msg in messageHistory)
        {
            // Clone the message to avoid JsonNode "already has a parent" error
            var clonedMsg = JsonNode.Parse(msg.ToJsonString())!.AsObject();
            messagesArray.Add(clonedMsg);
        }

        var request = new JsonObject
        {
            ["model"] = "llama-3.1-8b-instant",
            ["messages"] = messagesArray
        };

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

            if (!string.IsNullOrEmpty(content))
            {
                Debug.Log("Groq says: " + content);

                // Clone the assistant message too before storing
                var assistantMsg = new JsonObject
                {
                    ["role"] = "assistant",
                    ["content"] = content
                };
                messageHistory.Add(assistantMsg);
            }
        }
    }

}
