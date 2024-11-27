using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class findTexts : MonoBehaviour
{
    int btnNum = 0;
    public Button FindBtn;
    public GameObject TextBtn;
    public InputField inputText;
    private List<string> textFiles = new List<string> ();
    private string txtDirPath = Application.dataPath + "/AI/texts";

    void Start()
    {
        FindBtn.onClick.AddListener(FindBtnFunc);
    }

    void FindBtnFunc()
    {
        GetDirectoryFile();
    }

    public void InstantiateList()
    {
        TextBtn = GameObject.Find("textBtn");
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnHeight = 40; //Button�ĸ߶�
        int btnCount = textFiles.Count; //Button������
        float btnLength = 0;

        GameObject TxtContainer = GameObject.Find("TxtContainer");
        var ContainerRectTransform = TxtContainer.transform.GetComponent<RectTransform>();
        btnLength = ContainerRectTransform.rect.width - 10;
        
        GameObject TxtList = GameObject.Find("TxtList");
        var rectTransform = TxtList.transform.GetComponent<RectTransform>();
        TxtList.transform.localPosition = new Vector3(0, 0 - (btnHeight * btnCount) / 2 - rectTransform.rect.height / 2, 0);
        rectTransform.sizeDelta = new Vector2(btnLength, btnHeight * btnCount);

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
                    TextBtnFunc(text);    //���Ӱ�ť����¼�
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
        int btnPos = 0; //��һ��Button��Y��λ��
        int btnHeight = 80; //Button�ĸ߶�
        int btnCount = textTxt.Length; //Button������

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
        sendBtnRectTransform.sizeDelta = new Vector2(sendBtnRectTransform.rect.width - 10, sendBtnRectTransform.rect.height - 10);
        sendBtn.GetComponent<Outline>().effectDistance = new Vector2(5, 5);
        sendBtn.GetComponent<Outline>().effectColor = color;



        RemoveAllChildren(TxtTxtList);
        for (int i = 2; i < btnCount - 1; i++)
        {
            Debug.Log(textTxt[i]);
            string text = textTxt[i].Replace(" ","");

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
                    Click(txtName + text);    //���Ӱ�ť����¼�
                }
            );

            //��һ��Button��λ�õ��ڵ�ǰ��ȥ���ĸ߶�
            btnPos = btnPos - btnHeight;
        }
    }
    private void Click(string text)
    {
        text = text.Replace(" ","");
        int sepID = text.IndexOf('》');
        text = text.Substring(0, sepID + 1) + " " + text.Substring(sepID + 1, text.Length - sepID - 2);
        inputText.text = text;
        Debug.Log(text);
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
