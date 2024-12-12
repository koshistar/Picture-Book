using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using System.IO;


public class DeepSeekChat : MonoBehaviour
{
    public struct KeyWords
    {
        public int num;
        public string s;
    }
    // �滻Ϊ��� DeepSeek API key
    private string apiKey = "sk-195fb14227474c3183a972ba9710be31";
    private string apiUrl = "https://api.deepseek.com/chat/completions";
    public KeyWords k1,k2,k3;
    private string path;
    private string colorHex;
    Stack<KeyWords> st = new Stack<KeyWords>();

    // Unity UI Ԫ��
    public TMP_InputField userInputField;
    public TextMeshProUGUI chatOutputText;
    // ���ڴ洢�Ի���ʷ
    private List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();
    void Start()
    {
        // ��ʼ��ϵͳ��Ϣ
        path = Application.streamingAssetsPath+"/text";
        Debug.Log("datapath:" + path);
        k1.num = k2.num = k3.num = 0;
        k1.s = k2.s = k3.s = "";
        messages.Add(new Dictionary<string, string> { { "role", "system" }, { "content", "You are a helpful assistant." } });
    }

    public void updatekeywords()
    {
        int sum = k1.num + k2.num + k3.num;
        userInputField.text = "�봴��һ��";
        if (k1.num > 0) userInputField.text += k1.s + "Ϊ����";
        if (k2.num > 0) userInputField.text += "����" + k2.s + "Ϊ���ǵ��Ը�";
        if (k3.num > 0) userInputField.text += "����" + k3.s + "Ϊ����";
        userInputField.text += "�Ļ汾��";

        string gradientText = userInputField.text;
        colorHex = "";
        int p,t=3;
        while (t>0)
        {
            p = sum % 4;
            if (p > 0) colorHex += "FF";
            else colorHex += "00";
            sum /= 4;t--;
        }
        if (colorHex == "FFFF00") colorHex = "FFD700";
        if (colorHex == "00FF00") colorHex = "32B432";
        if (colorHex == "FFFFFF") colorHex = "000000";
        colorHex += "FF";
        userInputField.text = $"<color=#{colorHex}>"+gradientText+"</color>";
    }
    public void KeyWordButton11Clicked()
    {

        GameObject keyword = GameObject.Find("keyword1.1");
        k1.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k1.num = 1;
        updatekeywords();
    }

    public void KeyWordButton12Clicked()
    {
        GameObject keyword = GameObject.Find("keyword1.2");
        k1.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k1.num = 4;
        updatekeywords();
    }
    public void KeyWordButton13Clicked()
    {
        GameObject keyword = GameObject.Find("keyword1.3");
        k1.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k1.num = 16;
        updatekeywords();
    }
    public void KeyWordButton21Clicked()
    {
        GameObject keyword = GameObject.Find("keyword2.1");
        k2.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k2.num = 1;
        updatekeywords();
    }

    public void KeyWordButton22Clicked()
    {
        GameObject keyword = GameObject.Find("keyword2.2");
        k2.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k2.num = 4;
        updatekeywords();
    }
    public void KeyWordButton23Clicked()
    {
        GameObject keyword = GameObject.Find("keyword2.3");
        k2.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k2.num = 16;
        updatekeywords();
    }
    public void KeyWordButton31Clicked()
    {
        GameObject keyword = GameObject.Find("keyword3.1");
        k3.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k3.num = 1;
        updatekeywords();
    }

    public void KeyWordButton32Clicked()
    {
        GameObject keyword = GameObject.Find("keyword3.2");
        k3.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k3.num = 4;
        updatekeywords();
    }
    public void KeyWordButton33Clicked()
    {
        GameObject keyword = GameObject.Find("keyword3.3");
        k3.s = keyword.GetComponentInChildren<TMP_Text>().text;
        k3.num = 16;
        updatekeywords();
    }
    public void OnSendButtonClicked()
    {
        if (string.IsNullOrEmpty(userInputField.text)) return;
        chatOutputText.text += "Me: \n" + userInputField.text;
        k1.num = k2.num = k3.num = 0;
        k1.s = k2.s = k3.s = "";
        string userMessage = userInputField.text+ "�ϸ�Ҫ��ֻ�ظ��˻汾�ı���;籾�������һ��Ϊ���⣬ÿҳ�汾�ľ籾��һ��35�������ҵĻ�������ÿҳ�ľ���Ҫ�й�����ÿ�仰��ͷ�ð��������ֱ�ţ���ʮҳ������Ϊħ�����������й���Сѧ���꼶ѧ����";
        userInputField.text = "";

        // ����û���Ϣ���Ի���ʷ
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userMessage } });

        // ���� DeepSeek API
        StartCoroutine(CallDeepSeekAPI());  
    }

    private IEnumerator CallDeepSeekAPI()
    {
        string pre_chatOutputText = chatOutputText.text;
        chatOutputText.text += "\n��Ů���С�����";
        // ������������
        var requestData = new
        {
            model = "deepseek-chat",
            messages = messages,
            stream = false
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // ���� UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // ��������
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // ������Ӧ
            var response = JsonConvert.DeserializeObject<DeepSeekResponse>(request.downloadHandler.text);
            string botMessage = response.choices[0].message.content;

            // ��ʾ��Ӧ
            chatOutputText.text = pre_chatOutputText;
            chatOutputText.text += "\nAI: \n" + $"<color=#{colorHex}>" + botMessage + "</color>" + "\n";

            string title = "";
            for (int i = 0; i < botMessage.Length; i++)
                if (botMessage[i] != '\n')
                {
                    if (botMessage[i] != '*') title += botMessage[i];
                }
                else break;
            string Path;
            Path = path + "/" + title + ".txt";
            if (!File.Exists(Path))
            {
                Debug.Log("datapath:" + Path);
                FileStream fileStream = new FileStream(Path, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
                sw.WriteLine(botMessage+"\n"+colorHex);
                sw.Close();
                fileStream.Close();
            }
            // ��� AI ��Ϣ���Ի���ʷ
            messages.Add(new Dictionary<string, string> { { "role", "assistant" }, { "content", botMessage } });
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    [System.Serializable]
    public class DeepSeekResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string content;
    }

    void OnDisable()
    {

    }
}