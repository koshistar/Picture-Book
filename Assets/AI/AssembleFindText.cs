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
    private string txtDirPath;
   // public string FolderPath = "music"; // �ļ���·���������StreamingAssets�ļ���
    private AudioSource audioSource;
    private List<AudioClip> audioClips = new List<AudioClip>();
    private int currentClipIndex = 0;
    void Start()
    {
        txtDirPath = Application.persistentDataPath + "/text";
        AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, "guide8"));
        AudioManager.instance.PlayVoice(sound);
        FindBtn.onClick.AddListener(FindBtnFunc);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    IEnumerator LoadAudioClips(string txtName)
    {
        // ��ȡStreamingAssets�ļ��е�����·��
        string persistentDataPath =Application.persistentDataPath+ "/music" +'/'+txtName;

        Debug.Log(persistentDataPath);
        // ��ȡ�ļ����е������ļ�
        string[] files = System.IO.Directory.GetFiles(persistentDataPath, "*.mp3");
        // string[] files=Application.persistentDataPath + "/" + txtName;

        if (files.Length == 0)
        {
            Debug.LogError("No MP3 files found in folder: " + persistentDataPath);
            yield break;
        }

        // �����ļ���������Ƶ
        foreach (string file in files)
        {
            string url = "file://" + file; // �����ļ���URL
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest(); // ��������

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

        // ��ʼ���ŵ�һ����Ƶ
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

        // ���ŵ�ǰ��Ƶ�ļ�
        audioSource.clip = audioClips[currentClipIndex];
        audioSource.Play();

        // ������һ����Ƶ�ļ�������
        currentClipIndex = (currentClipIndex + 1) % audioClips.Count;

        // �ڵ�ǰ��Ƶ������Ϻ󣬲�����һ����Ƶ
        StartCoroutine(WaitForClipToFinish());
    }

    IEnumerator WaitForClipToFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // ��ǰ��Ƶ������Ϻ󣬲�����һ����Ƶ
        PlayNextClip();
    }
    void FindBtnFunc()
    {
        GetDirectoryFile();
    }

    public void InstantiateList()
    {
        TextBtn = GameObject.Find("textBtn");
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnHeight = 60; //Button�ĸ߶�
        int spaceHeight = 20; //Button֮��ļ��
        int btnCount = textFiles.Count; //Button������
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
            TextBtnClone.transform.localScale = new Vector3(1, 1, 1);    //���ڿ�¡��Button���ű�����Ϊ0����������Ҫ����Ϊ1
            TextBtnClone.transform.localPosition = new Vector3(0, btnPos, 0);
            var btn_rectTransform = TextBtnClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(btnLength, btnHeight);
            TextBtnClone.transform.Find("textBtnText").GetComponent<Text>().text = text;
            TextBtnClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    TextBtnFunc(text);    //��Ӱ�ť����¼�
                }
            );

            //��һ��Button��λ�õ��ڵ�ǰ��ȥ���ĸ߶�
            btnPos = btnPos - btnHeight;
        }
        btnNum = btnCount;
    }
    /// <summary>
    /// ����ļ��µ������ļ���
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
                //UnityEngine.Debug.Log("Name : " + textFiles[i].Name);//�ļ���
                Debug.Log("FullName : " + files[i].FullName);//��Ŀ¼�µ��ļ���Ŀ¼
                //UnityEngine.Debug.Log("DirectoryName : " + textFiles[i].DirectoryName);//��Ŀ¼
            }
            InstantiateList();
        }
    }

    private void TextBtnFunc(string txtName)
    {
        ReadText(txtName);
        StartCoroutine(LoadAudioClips(txtName)); // ����Э�̼�����Ƶ�ļ�
    }

    private void ReadText(string txtName)
    {
        string[] textTxt = File.ReadAllLines(txtDirPath + "/" + txtName + ".txt");

        GameObject TxtTxt = GameObject.Find("TxtTxt");
        
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnHeight = 80; //Button�ĸ߶�
        int btnCount = textTxt.Length; //Button������
        
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
            TxtTxtClone.transform.localScale = new Vector3(1, 1, 1);    //���ڿ�¡��Button���ű�����Ϊ0����������Ҫ����Ϊ1
            TxtTxtClone.transform.localPosition = new Vector3(0, btnPos, 0);
            var btn_rectTransform = TxtTxtClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(width, btnHeight);
            TxtTxtClone.transform.Find("TxtTxtText").GetComponent<Text>().text = text;
            TxtTxtClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    Click(txtName + text);    //��Ӱ�ť����¼�
                }
            );

            //��һ��Button��λ�õ��ڵ�ǰ��ȥ���ĸ߶�
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
        imgPath = Application.persistentDataPath + "/img/" + imgName + ".png";
        Debug.Log(imgPath);
        string imageStr="";
        if (!File.Exists(imgPath))
        {   
            imgPath = Application.persistentDataPath + "/img/white.png";
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
