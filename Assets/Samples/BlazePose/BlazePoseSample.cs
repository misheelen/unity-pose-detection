using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;
using System;

using System.Diagnostics;
using System.Collections;
using System.IO;

/// <summary>
/// BlazePose form MediaPipe
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/pose_tracking
/// </summary>
[RequireComponent(typeof(WebCamInput))]
public sealed class BlazePoseSample : MonoBehaviour
{

    [SerializeField]
    private BlazePose.Options options = default;

    [SerializeField]
    private RectTransform containerView = null;
    [SerializeField]
    private RawImage debugView = null;
    [SerializeField]
    private RawImage segmentationView = null;

    [SerializeField]
    private Canvas canvas = null;
    [SerializeField]
    private bool runBackground;
    [SerializeField, Range(0f, 1f)]
    private float visibilityThreshold = 0.5f;


    private BlazePose pose;
    private PoseDetect.Result poseResult;
    private PoseLandmarkDetect.Result landmarkResult;
    private BlazePoseDrawer drawer;

    private UniTask<bool> task;
    private CancellationToken cancellationToken;

    private readonly Vector4[] vportLandmarks;

    private string directoryName = @"C:\Users\Yamazaki-lab\Documents\Unity-projects\pose-unity-game\Recordings\captures\";
    private string fileName = "TestImage.png";


    private void Start()
    {
        pose = new BlazePose(options);

        drawer = new BlazePoseDrawer(Camera.main, gameObject.layer, containerView);

        cancellationToken = this.GetCancellationTokenOnDestroy();

        GetComponent<WebCamInput>().OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        GetComponent<WebCamInput>().OnTextureUpdate.RemoveListener(OnTextureUpdate);
        pose?.Dispose();
        drawer?.Dispose();
    }

    private void OnTextureUpdate(Texture texture)
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync(texture);
            }
        }
        else
        {
            Invoke(texture);
        }
    }

    private void Update()
    {
        drawer.DrawPoseResult(poseResult);

        bool rPoint = false;
        bool fPoint = false;
        bool lPoint = false;
        int sumPoint = 0;
        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            drawer.DrawCropMatrix(pose.CropMatrix);
            drawer.DrawLandmarkResult(landmarkResult, visibilityThreshold, canvas.planeDistance);

            // Marker and Pose position computation
            Vector4[] lmarks = landmarkResult.viewportLandmarks;
            Vector3 lmp = PointsChangeText.instance.LHM_transform.position;
            Vector3 rmp = PointsChangeText.instance.RHM_transform.position;
            Vector3 fmp = PointsChangeText.instance.FM_transform.position;
            
            for (int i = 0; i < lmarks.Length; i++)
            {
                Vector3 p = Camera.main.ViewportToWorldPoint(lmarks[i]);

                // check if face is inside the FMarker 
                if (i == 0 || i == 9 || i == 10) {
                    double resF = (p.x - fmp.x)*(p.x - fmp.x) + (p.y - fmp.y)*(p.y - fmp.y);   
                    if (resF <= 1) {
                        PointsChangeText.instance.FColor = 1;
                        if (!fPoint)
                        {
                            fPoint = true;
                            sumPoint += 10;
                            ScoreText.scoretxt.TextScore.text = String.Format("{0}", sumPoint); 
                        }
                    }
                    else {
                        if (!fPoint)
                        {
                            PointsChangeText.instance.FColor = 0;
                        }
                    }

                }

                // check if RIGHT hand is inside the RHMarker
                if (i == 16 || i == 18 || i == 20 || i == 22) {
                    double resR = (p.x - lmp.x)*(p.x - lmp.x) + (p.y - lmp.y)*(p.y - lmp.y);   
                    if (resR <= 1) {
                        PointsChangeText.instance.LHColor = 1;
                        if (!rPoint)
                        {
                            rPoint = true;
                            sumPoint += 10;
                            ScoreText.scoretxt.TextScore.text = String.Format("{0}", sumPoint); 
                        }
                    }
                    else 
                    {
                        if (!rPoint)
                        {
                            PointsChangeText.instance.LHColor = 0;
                        }
                    }
                }

                // check if LEFT hand is inside the LHMarker
                if (i == 15 || i == 17 || i == 19 || i == 21 ) {
                    double resL = (p.x - rmp.x)*(p.x - rmp.x) + (p.y - rmp.y)*(p.y - rmp.y);   
                    if (resL <= 1) {
                        PointsChangeText.instance.RHColor = 1;
                        if (!lPoint)
                        {
                            lPoint = true;
                            sumPoint = sumPoint + 10;
                            ScoreText.scoretxt.TextScore.text = String.Format("{0}", sumPoint); 
                        }
                    }
                    else 
                    {                        
                        if (!lPoint)
                        {
                            PointsChangeText.instance.RHColor = 0;
                        }
                    }
                }
                // vportLandmarks[i] = new Vector4(p.x, p.y, p.z + zOffset, landmarks[i].w);
                // Debug.Log(viewportLandmarks[i]);
            }
        
            if (options.landmark.useWorldLandmarks)
            {
                drawer.DrawWorldLandmarks(landmarkResult, visibilityThreshold);
            }
        }

        if (CountDown.instance.TakeScreenshot) 
        {
            TakeScreenshot();
            CountDown.instance.TakeScreenshot = false;
        }
    }

    private void Invoke(Texture texture)
    {
        landmarkResult = pose.Invoke(texture);
        poseResult = pose.PoseResult;
        if (pose.LandmarkInputTexture != null)
        {
            debugView.texture = pose.LandmarkInputTexture;
        }
        if (landmarkResult != null && landmarkResult.SegmentationTexture != null)
        {
            segmentationView.texture = landmarkResult.SegmentationTexture;
        }
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        landmarkResult = await pose.InvokeAsync(texture, cancellationToken);
        poseResult = pose.PoseResult;
        if (pose.LandmarkInputTexture != null)
        {
            debugView.texture = pose.LandmarkInputTexture;
        }
        if (landmarkResult != null && landmarkResult.SegmentationTexture != null)
        {
            segmentationView.texture = landmarkResult.SegmentationTexture;
        }
        return landmarkResult != null;
    }

    public void TakeScreenshot()
    {
        DirectoryInfo screenshotDirectory = Directory.CreateDirectory(directoryName);
        string fullPath = Path.Combine(screenshotDirectory.FullName, fileName);
        ScreenCapture.CaptureScreenshot(fullPath);
        PythonExecute();
    }

    private IEnumerable SaveImage(Texture camera_texture)
    {
        // UnityEngine.Debug.Log("Image has saved");

        yield return new WaitForEndOfFrame();

        //Create a Texture2D with the size of the rendered image on the screen.
        int width = Screen.width;
        int height = Screen.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Texture2D texture2D = camera_texture.ToTexture2D();
        
        //Save the image to the Texture2D
        // texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();

        //Encode it as a PNG.
        byte[] bytes = texture2D.EncodeToJPG();
        // Destroy(texture2D);
        
        //Save it in a file.
        File.WriteAllBytes(@"C:\Users\Yamazaki-lab\Documents\Unity-projects\pose-unity-game\Recordings\captures\" + "testimg.jpg", bytes);

        //Execute python script 
        PythonExecute();
    }

    private void PythonExecute() 
    {
        //pythonがある場所
        string pyExePath = @"C:\Users\Yamazaki-lab\anaconda3\envs\oc\python.exe";

        //実行したいスクリプトがある場所
        string pyCodePath = @"C:\Users\Yamazaki-lab\Documents\Unity-projects\img_comp\img_comp\img_comp.py";

        ProcessStartInfo processStartInfo = new ProcessStartInfo() {
            FileName = pyExePath, //実行するファイル(python)
            UseShellExecute = false,//シェルを使うかどうか
            CreateNoWindow = true, //ウィンドウを開くかどうか
            RedirectStandardOutput = true, //テキスト出力をStandardOutputストリームに書き込むかどうか
            Arguments = pyCodePath
            //Arguments = pyCodePath + " " + ID_INPUT_IMAGE_MOTIF, //実行するスクリプト 引数(複数可)
        };

        //外部プロセスの開始
        Process process = Process.Start(processStartInfo);

        //ストリームから出力を得る
        StreamReader streamReader = process.StandardOutput;
        string str = streamReader.ReadLine();

        //外部プロセスの終了
        process.WaitForExit();
        process.Close();

        //実行
        print(str);
        UnityEngine.Debug.Log("Python has executed");
    }
}
