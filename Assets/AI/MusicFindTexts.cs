using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class MusicFindTexts : MonoBehaviour
{
    public Button FindBtn;
    public GameObject TextBtn;
    public GameObject SongBtn;
    public InputField inputText;
    private int btnNum = 0;
    private string musicDirPath;
    private List<string> textFiles = new List<string> ();
    private List<string> musicFiles = new List<string>();
    private string txtDirPath = Application.streamingAssetsPath + "/text";
    private string rawMusicDirPath = Application.streamingAssetsPath + "/music";

    void Start()
    {
        FindBtn.onClick.AddListener(FindBtnFunc);
    }

    void FindBtnFunc()
    {
        GetTextDirectoryFile();
    }

    /// <summary>
    /// ����ļ��µ������ļ���
    /// </summary>
    private void GetTextDirectoryFile()
    {
        if (Directory.Exists(txtDirPath))
        {
            DirectoryInfo direction = new DirectoryInfo(txtDirPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            bool continueFlag = false;
            for (int i = 0; i <files.Length; i++)
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
            InstantiateTextList();
        }
    }
    public void InstantiateTextList()
    {
        TextBtn = GameObject.Find("textBtn");
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnHeight = 80; //Button�ĸ߶�
        int spaceHeight = 20; //Button֮��ļ��
        int btnCount = textFiles.Count; //Button������
        float btnLength = 0;

        GameObject TxtContainer = GameObject.Find("TxtContainer");
        var ContainerRectTransform = TxtContainer.transform.GetComponent<RectTransform>();
        btnLength = ContainerRectTransform.rect.width - 10;

        GameObject TxtList = GameObject.Find("TxtList");
        var rectTransform = TxtList.transform.GetComponent<RectTransform>();
        TxtList.transform.localPosition = new Vector3(0, 0 - (btnHeight + spaceHeight) * btnCount / 2 - rectTransform.rect.height / 2, 0);
        rectTransform.sizeDelta = new Vector2(btnLength, (btnHeight + spaceHeight) * btnCount);

        for (int i = btnNum; i < btnCount; i++)
        {
            string text = textFiles[i];
            string[] textTxt = File.ReadAllLines(txtDirPath + "/" + text + ".txt");
            string colorHex = textTxt[textTxt.Length - 1];
            UnityEngine.ColorUtility.TryParseHtmlString("#" + colorHex, out Color color);

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

    private void TextBtnFunc(string txtName)
    {
        ReadText(txtName);
        ReadMusic(txtName);
    }

    private void ReadText(string txtName)
    {
        string[] textTxt = File.ReadAllLines(txtDirPath + "/" + txtName + ".txt");
        string inputTxt = txtName;

        for (int i = 2; i < textTxt.Length - 1; i++)
        {
            Debug.Log(textTxt[i]);
            string text = textTxt[i].Replace(" ", "");
            inputTxt = inputTxt + '\n' + text;
        }
        string colorHex = textTxt[textTxt.Length - 1];
        UnityEngine.ColorUtility.TryParseHtmlString("#" + colorHex, out Color color);
        color.a = 0.5f;
        inputText.GetComponent<Image>().color = color;

        inputText.text = inputTxt;
    }

    private void ReadMusic(string txtName)
    {
        musicDirPath = rawMusicDirPath +  "/" + txtName;
        GetSongDirectoryFile();
    }

    /// <summary>
    /// ����ļ��µ������ļ���
    /// </summary>
    private void GetSongDirectoryFile()
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
                //UnityEngine.Debug.Log("Name : " + textFiles[i].Name);//�ļ���
                UnityEngine.Debug.Log("FullName : " + files[i].FullName);//��Ŀ¼�µ��ļ���Ŀ¼
                //UnityEngine.Debug.Log("DirectoryName : " + textFiles[i].DirectoryName);//��Ŀ¼
            }
            InstantiateSongList();
        }
        else
        {
            GameObject SongList = GameObject.Find("SongList");
            RemoveAllChildren(SongList);
        }
    }

    public void InstantiateSongList()
    {
        SongBtn = GameObject.Find("SongBtn");
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnLength = 150; //Button�ĳ���
        int spaceHeight = 50; //Button֮��ļ��
        int btnCount = musicFiles.Count; //Button������

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
            SongBtnClone.transform.localScale = new Vector3(1, 1, 1);    //���ڿ�¡��Button���ű�����Ϊ0����������Ҫ����Ϊ1
            SongBtnClone.transform.localPosition = new Vector3(btnPos, 0, 0);
            var btn_rectTransform = SongBtnClone.transform.GetComponent<RectTransform>();
            btn_rectTransform.sizeDelta = new Vector2(btnLength, btnLength);
            SongBtnClone.GetComponent<Button>().onClick.AddListener
            (
                () =>
                {
                    SongBtnFunc(SongBtnClone, SongList, musicFile);    //��Ӱ�ť����¼�
                }
            );

            //��һ��Button��λ�õ��ڵ�ǰ��ȥ���ĸ߶�
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
        StartCoroutine(Load(MusicBtn, musicDirPath + "/" + musicFile + ".mp3"));
        Debug.Log("Playing " + musicFile);
        
    }

    IEnumerator Load(GameObject MusicBtn, string audioName)
    {
        audioName = "file:///" + audioName;
        WWW ww = new WWW(audioName);
        yield return ww;
        if (ww.error == null && ww.isDone)
        {
            AudioClip audio = ww.GetAudioClip();
            AudioSource tar_music = MusicBtn.GetComponent<AudioSource>();
            tar_music.clip = audio;
            tar_music.Play();
            tar_music.loop = true;
        }
        else
        {
            Debug.Log(ww.error);
        }
    }
}
