using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Program : MonoBehaviour
{
    public RawImage showImage;
    public Text outcome;
    public InputField inputText;
    public Button SendBtn;
    private string APIexePath = Application.streamingAssetsPath + "/../qianfanAPI/qianfan_paint.exe";
    private string imgPath;
    private string imageStr;

    void Start()
    {
        SendBtn.onClick.AddListener(SendBtnFunc);
    }

    void SendBtnFunc()
    {
        imgPath = "";
        operate();
        imgPath = Application.streamingAssetsPath + "/img/" + inputText.text + ".png";
        imageStr = SetImageToString(imgPath);
        showImage.texture = GetTextureByString(imageStr);
    }

    void operate()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = APIexePath,
            Arguments = $"{inputText.text}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            outcome.text = output;
        }
    }
    /// <summary>
    /// 将图片转化为字符串
    /// </summary>
    private string SetImageToString(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        byte[] imgByte = new byte[fs.Length];
        fs.Read(imgByte, 0, imgByte.Length);
        fs.Close();
        return Convert.ToBase64String(imgByte);
    }

    /// <summary>
    /// 将字符串转换为纹理
    /// </summary>
    private Texture2D GetTextureByString(string textureStr)
    {
        Texture2D tex = new Texture2D(1, 1);
        byte[] arr = Convert.FromBase64String(textureStr);
        tex.LoadImage(arr);
        tex.Apply();
        return tex;
    }
}
