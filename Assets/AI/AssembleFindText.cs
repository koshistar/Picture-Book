using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssembleFindText : MonoBehaviour
{
    public RawImage showImage;
    int btnNum = 0;
    string wholeStory = "";
    public Button FindBtn;
    public GameObject TextBtn;
    public TextMeshProUGUI inputText;
    private Vector2 sendBtnWithOutline = Vector2.zero;
    private List<string> textFiles = new List<string>();
    private string txtDirPath = Application.streamingAssetsPath + "/text";
    public string folderPath = "music"; // 文件夹路径，相对于StreamingAssets文件夹
    private AudioSource audioSource;
    private List<AudioClip> audioClips = new List<AudioClip>();
    private int currentClipIndex = 0;
    void Start()
    {
        FindBtn.onClick.AddListener(FindBtnFunc);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    IEnumerator LoadAudioClips(string txtName)
    {
        // 获取StreamingAssets文件夹的完整路径
        string streamingAssetsPath = folderPath+'/'+txtName;

        Debug.Log(streamingAssetsPath);
        // 获取文件夹中的所有文件
        string[] files = System.IO.Directory.GetFiles(streamingAssetsPath, "*.mp3");

        if (files.Length == 0)
        {
            Debug.LogError("No MP3 files found in folder: " + streamingAssetsPath);
            yield break;
        }

        // 遍历文件并加载音频
        foreach (string file in files)
        {
            string url = "file://" + file; // 构建文件的URL
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest(); // 发送请求

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Failed to load audio file: " + file + "\nError: " + www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        audioClips.Add(clip);
                    }
                    else
                    {
                        Debug.LogError("Failed to load audio clip from file: " + file);
                    }
                }
            }
        }

        // 开始播放第一个音频
        if (audioClips.Count > 0)
        {
            PlayNextClip();
        }
        else
        {
            Debug.LogError("No valid audio clips loaded.");
        }
    }

    void PlayNextClip()
    {
        if (audioClips.Count == 0)
        {
            return;
        }

        // 播放当前音频文件
        audioSource.clip = audioClips[currentClipIndex];
        audioSource.Play();

        // 设置下一个音频文件的索引
        currentClipIndex = (currentClipIndex + 1) % audioClips.Count;

        // 在当前音频播放完毕后，播放下一个音频
        StartCoroutine(WaitForClipToFinish());
    }

    IEnumerator WaitForClipToFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // 当前音频播放完毕后，播放下一个音频
        PlayNextClip();
    }
    void FindBtnFunc()
    {
        GetDirectoryFile();
    }

    public void InstantiateList()
    {
        TextBtn = GameObject.Find("textBtn");
        int btnPos = 0; //第一个Button的Y轴位置
        int btnHeight = 60; //Button的高度
        int spaceHeight = 20; //Button之间的间隔
        int btnCount = textFiles.Count; //Button的数量
        float btnLength = 0;

        GameObject TxtContainer = GameObject.Find("TxtContainer");
        var ContainerRectTransform = TxtContainer.transform.GetComponent<RectTransform>();
        btnLength = ContainerRectTransform.rect.width - 10;

        GameObject TxtList = GameObject.Find("TxtList");
        var rectTransform = TxtList.transform.GetComponent<RectTransform>();
        Debug.Log(TxtList.transform.localPosition);
        TxtList.transform.localPosition = new Vector3(0, 0 - (((btnHeight + spaceHeight) * btnCount / 2) - (ContainerRectTransform.rect.height / 2)), 0);
        Debug.Log(TxtList.transform.localPosition);
        rectTransform.sizeDelta = new Vector2(btnLength, (btnHeight + spaceHeight) * btnCount);

        for (int i = btnNum; i < btnCount; i++)
        {
            string text = textFiles[i];
            string[] textTxt = File.ReadAllLines(txtDirPath + "/" + text + ".txt");
            string colorHex = textTxt[textTxt.Length - 1];
            ColorUtility.TryParseHtmlString("#" + colorHex, out Color color);

            //Debug.Log(rectTransform.rect.width);
            GameObject TextBtnClone = Instantiate(TextBtn);
            TextBtnClone.transform.SetParent(TxtList.transform);
            TextBtnClone.GetComponent<Outline>().effectColor = color;
            TextBtnClone.transform.localScale = new Vector3(1, 1, 1);    //由于克隆的Button缩放被设置为0，所以这里要设置为1
            TextBtnClone.transform.localPosition = new Vector3(0, btnPos, 0);
            var btn_rectTransform = TextBtnClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(btnLength, btnHeight);
            TextBtnClone.transform.Find("textBtnText").GetComponent<Text>().text = text;
            TextBtnClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    TextBtnFunc(text);    //添加按钮点击事件
                }
            );

            //下一个Button的位置等于当前减去他的高度
            btnPos = btnPos - btnHeight;
        }
        btnNum = btnCount;
    }
    /// <summary>
    /// 获得文件下的所有文件名
    /// </summary>
    private void GetDirectoryFile()
    {
        if (Directory.Exists(txtDirPath))
        {
            DirectoryInfo direction = new DirectoryInfo(txtDirPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            bool continueFlag = false;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continueFlag = true;
                }
                foreach (string text in textFiles)
                {
                    if (files[i].Name.Replace(".txt", "") == text)
                    {
                        continueFlag = true;
                        break;
                    }
                }
                if (continueFlag)
                {
                    continueFlag = false;
                    continue;
                }
                textFiles.Add(files[i].Name.Replace(".txt", ""));
                //UnityEngine.Debug.Log("Name : " + textFiles[i].Name);//文件名
                Debug.Log("FullName : " + files[i].FullName);//根目录下的文件的目录
                //UnityEngine.Debug.Log("DirectoryName : " + textFiles[i].DirectoryName);//根目录
            }
            InstantiateList();
        }
    }

    private void TextBtnFunc(string txtName)
    {
        ReadText(txtName);
        StartCoroutine(LoadAudioClips(txtName)); // 启动协程加载音频文件
    }

    private void ReadText(string txtName)
    {
        string[] textTxt = File.ReadAllLines(txtDirPath + "/" + txtName + ".txt");

        GameObject TxtTxt = GameObject.Find("TxtTxt");
        
        int btnPos = 0; //第一个Button的Y轴位置
        int btnHeight = 80; //Button的高度
        int btnCount = textTxt.Length; //Button的数量
        
        GameObject TxtTxtList = GameObject.Find("TxtTxtList");
        Debug.Log(btnCount);
        var rectTransform = TxtTxtList.transform.GetComponent<RectTransform>();
        TxtTxtList.transform.localPosition = new Vector3(0, 0 - (((btnHeight * btnCount) / 2) - (rectTransform.rect.height / 2)), 0);
        float width = rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(width, btnHeight * btnCount);
        string colorHex = textTxt[btnCount - 1];
        UnityEngine.ColorUtility.TryParseHtmlString("#" + colorHex, out Color color);
        color.a = 0.5f;
        TxtTxtList.GetComponent<Image>().color = color;
        color.a = 0.8f;
        
        wholeStory = "";
        RemoveAllChildren(TxtTxtList);
        for (int i = 2; i < btnCount - 1; i++)
        {
            Debug.Log(textTxt[i]);
            string text = textTxt[i].Replace(" ", "");
            wholeStory += text;
            GameObject TxtTxtClone = Instantiate(TxtTxt);
            TxtTxtClone.transform.SetParent(TxtTxtList.transform);
            TxtTxtClone.transform.localScale = new Vector3(1, 1, 1);    //由于克隆的Button缩放被设置为0，所以这里要设置为1
            TxtTxtClone.transform.localPosition = new Vector3(0, btnPos, 0);
            var btn_rectTransform = TxtTxtClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(width, btnHeight);
            TxtTxtClone.transform.Find("TxtTxtText").GetComponent<Text>().text = text;
            TxtTxtClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    Click(txtName + text);    //添加按钮点击事件
                }
            );

            //下一个Button的位置等于当前减去他的高度
            btnPos = btnPos - btnHeight;
        }
    }
    private string SetImageToString(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        byte[] imgByte = new byte[fs.Length];
        fs.Read(imgByte, 0, imgByte.Length);
        fs.Close();
        return Convert.ToBase64String(imgByte);
    }
    private Texture2D GetTextureByString(string textureStr)
    {
        Texture2D tex = new Texture2D(1, 1);
        byte[] arr = Convert.FromBase64String(textureStr);
        tex.LoadImage(arr);
        tex.Apply();
        return tex;
    }

    private void Click(string text)
    {
        text = text.Replace(" ", "");
        int sepID = text.IndexOf('》');
        text = text.Substring(0, sepID + 1) + " " + text.Substring(sepID + 1, text.Length - sepID - 2);
        inputText.text = text;
        Debug.Log(text);
        //inputText.text += ' ' + wholeStory;

        string imgPath = "";
        string imgName = inputText.text.Split(" ")[0] + '/' + inputText.text.Split(" ")[1];
        Debug.Log(imgName);
        imgPath = Application.streamingAssetsPath + "/img/" + imgName + ".png";
        Debug.Log(imgPath);
        string imageStr="";
        if (!File.Exists(imgPath))
        {   
            imgPath = Application.streamingAssetsPath + "/img/white.png";
            imageStr = SetImageToString(imgPath);
            Color color = showImage.color;
            color.a = 1;
            showImage.color = color;
            showImage.texture = GetTextureByString(imageStr);
        }
        else
        {
            imageStr = SetImageToString(imgPath);
            Color color = showImage.color;
            color.a = 1;
            showImage.color = color;
            showImage.texture = GetTextureByString(imageStr);
        }
        

        
    }

    public static void RemoveAllChildren(GameObject parent)
    {
        Transform transform;
        for (int i = 1; i < parent.transform.childCount; i++)
        {
            transform = parent.transform.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }
}
