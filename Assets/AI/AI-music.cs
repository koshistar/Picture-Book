using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AI_MUSIC : MonoBehaviour
{
    public Button SendBtn;
    public GameObject SongBtn;
    public InputField inputText;
    private string musicDirPath;
    private List<string> musicFiles = new List<string>();
    private string rawMusicDirPath;
    private string APIexePath = Application.streamingAssetsPath + "/foxAPI/foxai_music.exe";

    void Start()
    {
        rawMusicDirPath = Application.persistentDataPath + "/music";
        AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, "guide7"));
        AudioManager.instance.PlayVoice(sound);
        SendBtn.onClick.AddListener(SendBtnFunc);
    }

    void SendBtnFunc()
    {
        string prompt = inputText.text.Replace("\n", "");
        if (string.IsNullOrEmpty(prompt)) return;
        int sepID = prompt.IndexOf('.');
        musicDirPath = rawMusicDirPath + '/' + prompt.Substring(0, sepID - 1);
        UnityEngine.Debug.LogFormat("musicDirPath:{0}", musicDirPath);
        prompt = prompt.Substring(0, sepID - 1) + " " + prompt.Substring(sepID, prompt.Length - sepID);
        UnityEngine.Debug.LogFormat("prompt:{0}", prompt);
        Operate(prompt); 
        
    }

    private void Operate(string prompt)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = APIexePath,
            Arguments = $"{prompt + " " + rawMusicDirPath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = Process.Start(startInfo);
        process.EnableRaisingEvents = true;

        process.Exited += UpdateGameDataAfterProcessExit;
    }

    private void UpdateGameDataAfterProcessExit(object sender, EventArgs args)
    {
        Process process = (Process)sender;
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        UnityEngine.Debug.LogFormat("Music creating finished, output:{0}", output);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GetDirectoryFile();
        });
    }

    /// <summary>
    /// 获得文件下的所有文件名
    /// </summary>
    private void GetDirectoryFile()
    {
        if (Directory.Exists(musicDirPath))
        {
            DirectoryInfo direction = new DirectoryInfo(musicDirPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            musicFiles = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                musicFiles.Add(files[i].Name.Replace(".mp3", ""));
                //UnityEngine.Debug.Log("Name : " + textFiles[i].Name);//文件名
                UnityEngine.Debug.Log("FullName : " + files[i].FullName);//根目录下的文件的目录
                //UnityEngine.Debug.Log("DirectoryName : " + textFiles[i].DirectoryName);//根目录
            }
            InstantiateList();
        }
        else
        {
            GameObject SongList = GameObject.Find("SongList");
            RemoveAllChildren(SongList);
        }
    }

    public void InstantiateList()
    {
        SongBtn = GameObject.Find("SongBtn");
        int btnPos = 0; //第一个Button的Y轴位置
        int btnLength = 150; //Button的长宽
        int spaceHeight = 50; //Button之间的间隔
        int btnCount = musicFiles.Count; //Button的数量

        GameObject SongContainer = GameObject.Find("SongContainer");
        var ContainerRectTransform = SongContainer.transform.GetComponent<RectTransform>();

        GameObject SongList = GameObject.Find("SongList");
        var rectTransform = SongList.transform.GetComponent<RectTransform>();
        SongList.transform.localPosition = new Vector3(0 - ((btnLength + spaceHeight) * btnCount) / 2 - rectTransform.rect.height / 2, 0, 0);
        rectTransform.sizeDelta = new Vector2((btnLength + spaceHeight) * btnCount, btnLength);

        RemoveAllChildren(SongList);
        for (int i = 0; i < btnCount; i++)
        {
            string musicFile = musicFiles[i];
            //Debug.Log(rectTransform.rect.width);
            GameObject SongBtnClone = Instantiate(SongBtn);
            SongBtnClone.transform.SetParent(SongList.transform);
            SongBtnClone.transform.localScale = new Vector3(1, 1, 1);    //由于克隆的Button缩放被设置为0，所以这里要设置为1
            SongBtnClone.transform.localPosition = new Vector3(btnPos, 0, 0);
            var btn_rectTransform = SongBtnClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(btnLength, btnLength);
            SongBtnClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    SongBtnFunc(SongBtnClone, SongList, musicFile);    //添加按钮点击事件
                }
            );

            //下一个Button的位置等于当前减去他的高度
            btnPos = btnPos - btnLength / 2;
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

    void SongBtnFunc(GameObject MusicBtn, GameObject parent, string musicFile)
    {
        Transform transform;
        for (int i = 1; i < parent.transform.childCount; i++)
        {
            transform = parent.transform.GetChild(i);
            AudioSource audioSource = transform.gameObject.GetComponent<AudioSource>();
            audioSource.Stop();
        }
        AudioClip audio = Resources.Load<AudioClip>(musicDirPath + "/" + musicFile + ".mp3");
        AudioSource tar_music = MusicBtn.GetComponent<AudioSource>();
        tar_music.clip = audio;
        tar_music.Play();
        tar_music.loop = true;
        UnityEngine.Debug.Log("Playing " + musicFile);
    }
}
