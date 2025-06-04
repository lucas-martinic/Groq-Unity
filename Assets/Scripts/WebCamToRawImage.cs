using UnityEngine;
using UnityEngine.UI;

public class WebCamToRawImage : MonoBehaviour
{
    public RawImage rawImage;          // Assign this in the Inspector
    private WebCamTexture webCamTexture;

    void Start()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogWarning("No webcam detected.");
            return;
        }

        WebCamDevice device = WebCamTexture.devices[0]; // You can add a dropdown to select others
        webCamTexture = new WebCamTexture(device.name);

        rawImage.texture = webCamTexture;
        rawImage.material.mainTexture = webCamTexture;

        webCamTexture.Play();
    }

    void OnDisable()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
