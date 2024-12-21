using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AI_PAINT : MonoBehaviour
{
    public RawImage showImage;
    public InputField inputText;
    public Button SendBtn;
    private string APIexePath = Application.dataPath + "/qianfanAPI/qianfan_paint.exe";
    private string imgPath;
    private string imageStr;

    void Start()
    {
        	
        AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, "guide6"));
        AudioManager.instance.PlayVoice(sound);
            
        SendBtn.onClick.AddListener(SendBtnFunc);
    }

    void SendBtnFunc()
    {
        imgPath = "";
        operate();

        GameData.hasFinished = true;
        GameData.CD -= 0.1f;
        GameData.hasGenerateImage = true;
        
        string imgName = inputText.text.Split(" ")[0] + '/' + inputText.text.Split(" ")[1];
        imgPath = Application.streamingAssetsPath + "/img/" + imgName + ".png";
        imageStr = SetImageToString(imgPath);

        Color color = showImage.color;
        color.a = 1;
        showImage.color = color;
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

            //process.WaitForExit();
            UnityEngine.Debug.LogFormat("Painting finished, output:{0}", output);
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
