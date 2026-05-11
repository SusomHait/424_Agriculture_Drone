using System.Collections; 
using System.IO;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using TMPro;

public class ImageCapture : MonoBehaviour
{
    public Camera main_camera;
    public Camera viewport_camera;
    public Camera image_camera;

    public Button capture_btn;

    private int image_capture_mode;

    void Awake()
    {
        main_camera.enabled = false; 
        viewport_camera.enabled = false;
        image_camera.enabled = false;

        // retoggle the cameras to make them show up properly
        main_camera.enabled = true; 
        viewport_camera.enabled = true;

        image_capture_mode = PlayerPrefs.GetInt("ImageCaptureMode", 0);

        capture_btn.interactable = true; 
        // if auto mode --> turn the btn off and start the capture
        if (image_capture_mode == 1)
        {
            capture_btn.interactable = false;
            StartCoroutine(AutomaticCapture());
        }

        Debug.Log(Application.persistentDataPath);
    }

    public void TakeImage()
    {
        int width = 512;
        int height = 512;

        // define a texture and capture the image from the camera
        RenderTexture rt = new RenderTexture(width, height, 24);
        image_camera.targetTexture = rt;

        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);

        image_camera.Render();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        image_camera.targetTexture = null;
        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();
        string folder = Path.Combine(Application.persistentDataPath, "Images");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        string path = Path.Combine(folder, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png");

        File.WriteAllBytes(path, bytes);

        rt.Release();
        Destroy(rt);
        Destroy(image);

        Debug.Log("Saved image to: " + path);
    }

    // use a thread to track and take pictures every 5 seconds
    IEnumerator AutomaticCapture()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            TakeImage();
        }
    }

    void Update()
    {
        // capture button is t
        if (image_capture_mode == 0)
        {
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                TakeImage();
            }
        }
    }
}