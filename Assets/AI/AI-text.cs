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
    // 替换为你的 DeepSeek API key
    private string apiKey = "sk-195fb14227474c3183a972ba9710be31";
    private string apiUrl = "https://api.deepseek.com/chat/completions";
    public KeyWords k1, k2, k3;
    private string path;
    private string colorHex;
    private string originask;
    private string position;

    // Unity UI 元素
    public TMP_InputField userInputField;
    public TextMeshProUGUI chatOutputText;
    public TextMeshProUGUI AskingText;
    public TextMeshProUGUI k1_1, k1_2, k1_3, k2_1, k2_2, k2_3, k3_1, k3_2, k3_3;

    // 用于存储对话历史
    private List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();
    private List<Dictionary<string, string>> messages1 = new List<Dictionary<string, string>>();
    void Start()
    {
        AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, "guide5")); 
        AudioManager.instance.PlayVoice(sound);
        
        // 初始化系统消息
        originask = AskingText.text;
        position = "";
        path = Application.persistentDataPath + "/text";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("create datapath:" + path);
        }
        else
            Debug.Log("datapath:" + path);
        k1.num = k2.num = k3.num = 0;
        k1.s = k2.s = k3.s = "";
        messages.Add(new Dictionary<string, string> { { "role", "system" }, { "content", "You are a helpful assistant." } });
    }

    public void updatekeywords()
    {
        int sum = k1.num + k2.num + k3.num;
        userInputField.text = "请创作一个";
        if (k1.num > 0) userInputField.text += k1.s + "为主角";
        if (k2.num > 0) userInputField.text += "，以" + k2.s + "为主角的性格";
        if (k3.num > 0) userInputField.text += "，以" + k3.s + "为主题";
        userInputField.text += "的绘本。";

        string gradientText = userInputField.text;
        colorHex = "";
        int p, t = 3;
        while (t > 0)
        {
            p = sum % 4;
            if (p > 0) colorHex += "FF";
            else colorHex += "00";
            sum /= 4; t--;
        }
        if (colorHex == "FFFF00") colorHex = "FFD700";
        if (colorHex == "00FF00") colorHex = "32B432";
        if (colorHex == "FFFFFF") colorHex = "000000";
        colorHex += "FF";
        userInputField.text = $"<color=#{colorHex}>" + gradientText + "</color>";
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

    public void SceneButton1Clicked()
    {
        AskingText.text = originask;
        GameObject scene = GameObject.Find("scene1");
        AskingText.text += scene.GetComponentInChildren<TMP_Text>().text;
        position = scene.GetComponentInChildren<TMP_Text>().text;
    }

    public void SceneButton2Clicked()
    {
        AskingText.text = originask;
        GameObject scene = GameObject.Find("scene2");
        AskingText.text += scene.GetComponentInChildren<TMP_Text>().text;
        position = scene.GetComponentInChildren<TMP_Text>().text;
    }

    public void SceneButton3Clicked()
    {
        AskingText.text = originask;
        GameObject scene = GameObject.Find("scene3");
        AskingText.text += scene.GetComponentInChildren<TMP_Text>().text;
        position = scene.GetComponentInChildren<TMP_Text>().text;
    }

    public void SceneButton4Clicked()
    {
        AskingText.text = originask;
        GameObject scene = GameObject.Find("scene4");
        AskingText.text += scene.GetComponentInChildren<TMP_Text>().text;
        position = scene.GetComponentInChildren<TMP_Text>().text;
    }

    public void SceneButton5Clicked()
    {
        AskingText.text = originask;
        GameObject scene = GameObject.Find("scene5");
        AskingText.text += scene.GetComponentInChildren<TMP_Text>().text;
        position = scene.GetComponentInChildren<TMP_Text>().text;
    }
    public void OnSendButtonClicked()
    {
        if (string.IsNullOrEmpty(userInputField.text)) return;
        chatOutputText.text += "Me: \n" + userInputField.text;
        k1.num = k2.num = k3.num = 0;
        k1.s = k2.s = k3.s = "";
        string userMessage = userInputField.text + "严格要求只回复此绘本的标题和剧本，输出第一行为标题，每页绘本的剧本用一句35个字左右的话描述，每页的剧情要有关联，每句话开头用阿拉伯数字标号，共十页，背景为魔法海洋，面向中国的小学二年级学生。";
        userInputField.text = "";

        // 添加用户消息到对话历史
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userMessage } });

        // 调用 DeepSeek API
        StartCoroutine(CallDeepSeekAPI());
    }

    private IEnumerator CallDeepSeekAPI()
    {
        string pre_chatOutputText = chatOutputText.text;
        chatOutputText.text += "\n少女祈祷中。。。";
        // 创建请求数据
        var requestData = new
        {
            model = "deepseek-chat",
            messages = messages,
            stream = false
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // 创建 UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 发送请求
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 解析响应
            var response = JsonConvert.DeserializeObject<DeepSeekResponse>(request.downloadHandler.text);
            string botMessage = response.choices[0].message.content;
            
            GameData.hasFinished = true;
            GameData.CD -= 0.1f;
            GameData.hasGenerateText = true;
            
            // 显示响应
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
                sw.WriteLine(botMessage + "\n" + colorHex);
                sw.Close();
                fileStream.Close();
            }
            // 添加 AI 消息到对话历史
            messages.Add(new Dictionary<string, string> { { "role", "assistant" }, { "content", botMessage } });
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void ConfirmButtonClicked()
    {
        if (string.IsNullOrEmpty(position)) return;

        string userMessage = "请给出3个三个字及以下的生活在" + position + "的主角的生物物种，以及他们的性格和他们故事的主题，共9个词，用空格分开，前3个为主角，中间3个为性格，最后3个为主题，只输出这9个词，每个词中的汉字不允许复杂，保证小学二年级学生能够阅读。";

        // 添加用户消息到对话历史
        messages1.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userMessage } });

        // 调用 DeepSeek API
        StartCoroutine(CallDeepSeekAPI1());
    }

    private IEnumerator CallDeepSeekAPI1()
    {
        // 创建请求数据
        var requestData = new
        {
            model = "deepseek-chat",
            messages = messages1,
            stream = false
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // 创建 UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 发送请求
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 解析响应
            var response = JsonConvert.DeserializeObject<DeepSeekResponse>(request.downloadHandler.text);
            string botMessage = response.choices[0].message.content;
            Debug.Log(botMessage);
            int i = 0;
            string sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k1_1.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k1_2.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k1_3.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k2_1.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k2_2.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k2_3.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k3_1.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k3_2.text = sk;

            sk = "";
            while (i < botMessage.Length && botMessage[i] != ' ') sk += botMessage[i++];
            ++i;
            k3_3.text = sk;

            messages1.Add(new Dictionary<string, string> { { "role", "assistant" }, { "content", botMessage } });
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