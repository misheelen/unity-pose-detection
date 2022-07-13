// This script is not used in the main scene.  
// Capturing image from web camera
// reference: 
// https://docs.unity3d.com/ScriptReference/Windows.WebCam.PhotoCapture.TakePhotoAsync.html#:~:text=Asynchronously%20captures%20a%20photo%20from%20the%20web%20camera,pass%20the%20image%20data%20to%20an%20external%20plugin. 

using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class PhotoCaptureWebcam : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;

    static readonly int TotalImagesToCapture = 3;
    int capturedImageCount = 0;
    string local_path = @"C:\Users\Yamazaki-lab\Documents\Unity-projects\pose-unity-game\Recordings\captures";

    // Use this for initialization
    void Start()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        Debug.Log("Start Photo Capture");

        PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
            Debug.Log("Created PhotoCapture Object");
            photoCaptureObject = captureObject;

            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 0.0f;
            c.cameraResolutionWidth = targetTexture.width;
            c.cameraResolutionHeight = targetTexture.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;

            captureObject.StartPhotoModeAsync(c, delegate(PhotoCapture.PhotoCaptureResult result) {
                Debug.Log("Started Photo Capture Mode");
                TakePicture();
            });
        });
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Saved Picture To Disk!");

        if (capturedImageCount < TotalImagesToCapture)
        {
            TakePicture();
        }
        else
        {
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }

    void TakePicture()
    {
        capturedImageCount++;
        Debug.Log(string.Format("Taking Picture ({0}/{1})...", capturedImageCount, TotalImagesToCapture));
        string filename = string.Format(@"CapturedImage{0}.jpg", capturedImageCount);
        // string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
        string filePath = System.IO.Path.Combine(local_path, filename);
        Debug.Log(filename);
        try
        {
            photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);    
        }
        catch (System.Exception)
        {
            Debug.Log("上の2つ以外の例外です。");
        }
        
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;

        Debug.Log("Captured images have been saved at the following path.");
        Debug.Log(Application.persistentDataPath);
    }
}