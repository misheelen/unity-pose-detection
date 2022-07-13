using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Additional packages
// referenced from: 
// https://answers.unity.com/questions/337530/how-to-save-a-snapshot-of-the-webcamtexture.html

using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace TensorFlowLite
{
    /// <summary>
    /// An wrapper for WebCamTexture that corrects texture rotation
    /// </summary>
    public sealed class WebCamInput : MonoBehaviour
    {
        [System.Serializable]
        public class TextureUpdateEvent : UnityEvent<Texture> { }

        [SerializeField, WebCamName] private string editorCameraName;
        [SerializeField] private WebCamKind preferKind = WebCamKind.WideAngle;
        [SerializeField] private bool isFrontFacing = false;
        [SerializeField] private Vector2Int requestSize = new Vector2Int(1280, 720);
        [SerializeField] private int requestFps = 60;
        public TextureUpdateEvent OnTextureUpdate = new TextureUpdateEvent();

        private TextureResizer resizer;
        private WebCamTexture webCamTexture = null;
        private WebCamDevice[] devices;
        private int deviceIndex;

        private void Start()
        {
            resizer = new TextureResizer();
            devices = WebCamTexture.devices;
            string cameraName = Application.isEditor
                ? editorCameraName
                : WebCamUtil.FindName(preferKind, isFrontFacing);

            WebCamDevice device = default;
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].name == cameraName)
                {
                    device = devices[i];
                    deviceIndex = i;
                    break;
                }
            }

            StartCamera(device);
        }

        private void OnDestroy()
        {
            StopCamera();
            resizer?.Dispose();
        }

        private void Update()
        {
            if (!webCamTexture.didUpdateThisFrame) return;

            // if(Input.GetMouseButtonDown(0))
            // if (CountDown.instance.CountDownFlag == 1)
            // {
            //     SaveImage();
            // }

            var tex = NormalizeWebcam(webCamTexture, Screen.width, Screen.height, isFrontFacing);
            OnTextureUpdate.Invoke(tex);
        }

        // Invoked by Unity Event
        [Preserve]
        public void ToggleCamera()
        {
            deviceIndex = (deviceIndex + 1) % devices.Length;
            StartCamera(devices[deviceIndex]);
        }

        private void StartCamera(WebCamDevice device)
        {
            StopCamera();
            isFrontFacing = device.isFrontFacing;
            webCamTexture = new WebCamTexture(device.name, requestSize.x, requestSize.y, requestFps);

            // Additional code /start/
            //Render the image in the screen.
            // Debug.Log(webCamTexture);
            // rawimage.texture = null;
            // rawimage.material.mainTexture = null;
            // Additional code /end/

            webCamTexture.Play();
        }

        private void StopCamera()
        {
            if (webCamTexture == null)
            {
                return;
            }
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }

        private RenderTexture NormalizeWebcam(WebCamTexture texture, int width, int height, bool isFrontFacing)
        {
            int cameraWidth = texture.width;
            int cameraHeight = texture.height;
            bool isPortrait = IsPortrait(texture);
            if (isPortrait)
            {
                (cameraWidth, cameraHeight) = (cameraHeight, cameraWidth); // swap
            }

            float cameraAspect = (float)cameraWidth / cameraHeight;
            float targetAspect = (float)width / height;

            int w, h;
            if (cameraAspect > targetAspect)
            {
                w = RoundToEven(cameraHeight * targetAspect);
                h = cameraHeight;
            }
            else
            {
                w = cameraWidth;
                h = RoundToEven(cameraWidth / targetAspect);
            }

            Matrix4x4 mtx;
            Vector4 uvRect;
            int rotation = texture.videoRotationAngle;

            // Seems to be bug in the android. might be fixed in the future.
            if (Application.platform == RuntimePlatform.Android)
            {
                rotation = -rotation;
            }

            if (isPortrait)
            {
                mtx = TextureResizer.GetVertTransform(rotation, texture.videoVerticallyMirrored, isFrontFacing);
                uvRect = TextureResizer.GetTextureST(targetAspect, cameraAspect, AspectMode.Fill);
            }
            else
            {
                mtx = TextureResizer.GetVertTransform(rotation, isFrontFacing, texture.videoVerticallyMirrored);
                uvRect = TextureResizer.GetTextureST(cameraAspect, targetAspect, AspectMode.Fill);
            }

            // Debug.Log($"camera: rotation:{texture.videoRotationAngle} flip:{texture.videoVerticallyMirrored}");
            return resizer.Resize(texture, w, h, false, mtx, uvRect);
        }

        private static bool IsPortrait(WebCamTexture texture)
        {
            return texture.videoRotationAngle == 90 || texture.videoRotationAngle == 270;
        }

        private static int RoundToEven(float n)
        {
            return Mathf.RoundToInt(n / 2) * 2;
        }

        private void SaveImage()
        {
            //Create a Texture2D with the size of the rendered image on the screen.
            Texture2D texture = new Texture2D(requestSize.x, requestSize.y, TextureFormat.ARGB32, false);
            
            //Save the image to the Texture2D
            texture.SetPixels(webCamTexture.GetPixels());
            texture.Apply();

            //Encode it as a PNG.
            byte[] bytes = texture.EncodeToPNG();
            
            //Save it in a file.
            File.WriteAllBytes(Application.persistentDataPath + "testimg.png", bytes);
        }
    }
}
