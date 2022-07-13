using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CsPy : MonoBehaviour {
    //pythonがある場所
    private string pyExePath = @"C:\Users\Yamazaki-lab\anaconda3\envs\oc\python.exe";

    //実行したいスクリプトがある場所
    private string pyCodePath = @"C:\Users\Yamazaki-lab\Documents\Unity-projects\img_comp\img_comp\img_comp.py";

    private void Start () {
        //外部プロセスの設定
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
    }
}