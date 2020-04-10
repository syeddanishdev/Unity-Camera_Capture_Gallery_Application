using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NativeCam : MonoBehaviour
{
    [SerializeField] GameObject uiContent;
    [SerializeField] GameObject captureButton;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI departmentText;
    [SerializeField] GameObject detailsContent;
    [SerializeField] GameObject controllButtons;
    [SerializeField] TextMeshProUGUI detailsText;
    [SerializeField] GameObject ImageHolder;


    void Awake()
    {
        showMainMenu();
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            // Don't attempt to use the camera if it is already open
            if (NativeCamera.IsCameraBusy())
                return;

            if (Input.mousePosition.x < Screen.width / 2)
            {
                // Take a picture with the camera
                // If the captured image's width and/or height is greater than 512px, down-scale it
                TakePicture(512);
            }
            else
            {
                // Record a video with the camera
                RecordVideo();
            }
        }
    }
    public void ClickPicture()
    {
        TakePicture(1440);
    }
    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                ImageHolder.GetComponent<RawImage>().texture = texture;
                ImageHolder.SetActive(true);
                /*
                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;
				*/
                uiContent.SetActive(true);
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }
    public void showMainMenu()
    {
        captureButton.SetActive(true);
        uiContent.SetActive(false);
        detailsContent.SetActive(false);
        ImageHolder.SetActive(false);
    }
    public void confirmData()
    {
        uiContent.SetActive(false);
        string result = "Name : " + nameText.text + "\nDepartment : " + departmentText.text;
        detailsText.text = result;
        detailsContent.SetActive(true);
        controllButtons.SetActive(true);
    }
    public void backfrmConfirmScreen()
    {
        detailsContent.SetActive(false);
        controllButtons.SetActive(false);
        uiContent.SetActive(true);
    }
    public void saveData()
    {
        controllButtons.SetActive(false);
        StartCoroutine(TakeScreenshotAndSave());
    }
    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "MyGallery", "Image.png"));
        // To avoid memory leaks
        Destroy(ss);
        showMainMenu();
    }

    private void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the recorded video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        });

        Debug.Log("Permission result: " + permission);
    }
}
