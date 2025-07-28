# Groq-Unity 🧠🎮

**Groq-Unity** is an open-source Unity integration for Groq's AI APIs.

> 🚀 Built on top of [jgravelle/GroqApiLibrary](https://github.com/jgravelle/GroqApiLibrary), fully adapted for Unity with samples and MonoBehaviour support.

---

## ✨ Features

- ✅ **Chat Completion** with Mixtral / LLaMA models
- 🎙️ **Speech Synthesis (TTS)** using `playai-tts` voices
- 🧾 **Transcription** using `whisper-large-v3`
- 🧮 **Function Calling & Tool Use** from within Unity
- 🔊 Built-in audio player for TTS & transcription demos
- 🔧 Simple MonoBehaviour components for easy use in scenes

---

## 📦 Requirements

- Unity 6+(tested)
- A valid [Groq API Key](https://console.groq.com/)

---

## 🔧 Setup

1. Clone or download this repository
2. (Optional) If you want are integrating these scripts into your project, install [NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity) and the packages: "System.Net.Http.Json"
3. Add your Groq API Key in the relevant script or config field
4. Open the sample scene, enable one of the 5 sample gameobjects, hit play.
5. For TTS sample, go to https://console.groq.com/playground?model=playai-tts and accept the terms.

---

## 🧪 Samples Included

| Feature        | Scene / Script                    | Description |
|----------------|-----------------------------------|-------------|
| 🔡 Chat         | `GroqChat.cs`                     | Simple text prompt and response using Mixtral |
| 🗣️ TTS           | `GroqTTS.cs`                      | Uses `playai-tts` to generate speech from text and play it |
| 🎧 Transcription| `WhisperTranscriber.cs`          | Record mic input and transcribe with `whisper-large-v3` |
| 🧰 Tool Calling | `GroqToolSample.cs`              | Run a tool (e.g. spawn colored cubes) via structured function call |
| 🎛️ Voice Enum   | `GroqTTS.cs`                 | Enum of all available voices for TTS selection in the Editor |

---

