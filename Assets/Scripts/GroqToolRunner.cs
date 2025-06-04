using GroqApiLibrary;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Text.Json;

using UnityEngine;
using JsonArray = System.Text.Json.Nodes.JsonArray;
using JsonObject = System.Text.Json.Nodes.JsonObject;

public class GroqToolRunner : MonoBehaviour
{
    [SerializeField] private string apiKey = "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";
    [SerializeField] private string prompt = "Spawn a cube and make it red.";
    [SerializeField] private string model = "mixtral-8x7b-32768";
    private GroqApiClient groqApi;
    private List<Tool> tools;

    private void Start()
    {
        groqApi = new GroqApiClient(apiKey);
        tools = new List<Tool> { BuildSpawnCubeTool(), BuildSpawnSphereTool() };
    }

    [Button]
    async void TestPromptWithTools()
    {
        string systemMessage = "You are a helpful assistant that can execute Unity commands.";
        string result = await groqApi.RunConversationWithToolsAsync(prompt, tools, model, systemMessage);
        Debug.Log("Response: " + result);
    }

    private Tool BuildSpawnCubeTool()
    {
        return new Tool
        {
            Type = "function",
            Function = new Function
            {
                Name = "spawn_cube",
                Description = "Spawns a Unity cube and colors it",
                Parameters = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["color"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["description"] = "Color name"
                        }
                    },
                    ["required"] = new JsonArray { "color" }
                },
                ExecuteAsync = async (args) =>
                {
                    var jsonArgs = JsonDocument.Parse(args);
                    var color = jsonArgs.RootElement.GetProperty("color").GetString();

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = Vector3.zero;

                        if (ColorUtility.TryParseHtmlString(color, out var parsedColor))
                        {
                            cube.GetComponent<Renderer>().material.color = parsedColor;
                        }
                        else if (TryGetColorByName(color, out var namedColor))
                        {
                            cube.GetComponent<Renderer>().material.color = namedColor;
                        }
                    });

                    return JsonSerializer.Serialize(new { result = $"Cube created with color {color}" });
                }
            }
        };
    }

    private Tool BuildSpawnSphereTool()
    {
        return new Tool
        {
            Type = "function",
            Function = new Function
            {
                Name = "spawn_sphere",
                Description = "Spawns a Unity sphere and colors it",
                Parameters = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["color"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["description"] = "Color name"
                        }
                    },
                    ["required"] = new JsonArray { "color" }
                },
                ExecuteAsync = async (args) =>
                {
                    var jsonArgs = JsonDocument.Parse(args);
                    var color = jsonArgs.RootElement.GetProperty("color").GetString();

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        cube.transform.position = Vector3.zero;

                        if (ColorUtility.TryParseHtmlString(color, out var parsedColor))
                        {
                            cube.GetComponent<Renderer>().material.color = parsedColor;
                        }
                        else if (TryGetColorByName(color, out var namedColor))
                        {
                            cube.GetComponent<Renderer>().material.color = namedColor;
                        }
                    });

                    return JsonSerializer.Serialize(new { result = $"Cube created with color {color}" });
                }
            }
        };
    }

    private static bool TryGetColorByName(string name, out Color color)
    {
        switch (name.ToLower())
        {
            case "red": color = Color.red; return true;
            case "blue": color = Color.blue; return true;
            case "green": color = Color.green; return true;
            case "yellow": color = Color.yellow; return true;
            case "white": color = Color.white; return true;
            case "black": color = Color.black; return true;
            case "cyan": color = Color.cyan; return true;
            case "magenta": color = Color.magenta; return true;
            default: color = Color.white; return false;
        }
    }
}
