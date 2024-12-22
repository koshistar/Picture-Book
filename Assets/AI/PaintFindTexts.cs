using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class PaintFindTexts : MonoBehaviour
{
    int btnNum = 0;
    string wholeStory = "";
    public Button FindBtn;
    public GameObject TextBtn;
    public InputField inputText;
    private Vector2 sendBtnWithOutline = Vector2.zero;
    private List<string> textFiles = new List<string> ();
    private string txtDirPath;

    void Start()
    {
        FindBtn.onClick.AddListener(FindBtnFunc);
        if (sendBtnWithOutline.Equals(Vector2.zero))
        {
            txtDirPath = Application.persistentDataPath + "/text";
            GameObject sendBtn = GameObject.Find("sendBtn");
            var sendBtnRectTransform = sendBtn.transform.GetComponent<RectTransform>();
            sendBtnWithOutline = new Vector2(sendBtnRectTransform.rect.width - 10, sendBtnRectTransform.rect.height - 10);
        }
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
    }

    private void ReadText(string txtName)
    {
        string[] textTxt = File.ReadAllLines(txtDirPath + "/" + txtName + ".txt");

        GameObject TxtTxt = GameObject.Find("TxtTxt");
        int btnPos = 0; //第一个Button的Y轴位置
        int btnHeight = 80; //Button的高度
        int btnCount = textTxt.Length; //Button的数量

        GameObject TxtTxtList = GameObject.Find("TxtTxtList");
        var rectTransform = TxtTxtList.transform.GetComponent<RectTransform>();
        TxtTxtList.transform.localPosition = new Vector3(0, 0 - (((btnHeight * btnCount) / 2) - (rectTransform.rect.height / 2)), 0);
        float width = rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(width, btnHeight * btnCount);
        string colorHex = textTxt[btnCount - 1];
        UnityEngine.ColorUtility.TryParseHtmlString("#" + colorHex, out Color color);
        color.a = 0.5f;
        TxtTxtList.GetComponent<Image>().color = color;
        color.a = 0.8f;
        GameObject sendBtn = GameObject.Find("sendBtn");
        var sendBtnRectTransform = sendBtn.transform.GetComponent<RectTransform>();
        sendBtnRectTransform.sizeDelta = sendBtnWithOutline;
        sendBtn.GetComponent<Outline>().effectDistance = new Vector2(5, 5);
        sendBtn.GetComponent<Outline>().effectColor = color;

        wholeStory = "";
        RemoveAllChildren(TxtTxtList);
        for (int i = 2; i < btnCount - 1; i++)
        {
            Debug.Log(textTxt[i]);
            string text = textTxt[i].Replace(" ","");
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
    private void Click(string text)
    {
        text = text.Replace(" ","");
        int sepID = text.IndexOf('.');
        text = text.Substring(0, sepID - 1) + " " + text.Substring(sepID - 1, text.Length - sepID);
        inputText.text = text;
        Debug.Log(text);
        inputText.text += ' ' + wholeStory;
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
