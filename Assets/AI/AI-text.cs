using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using TMPro;

public class DeepSeekChat : MonoBehaviour
{
    // �滻Ϊ��� DeepSeek API key
    private string apiKey = "sk-4eb1bc9a91574257a0fde8c4df6e4f82";
    private string apiUrl = "https://api.deepseek.com/chat/completions";

    // Unity UI Ԫ��
    public TMP_InputField userInputField;
    public TextMeshProUGUI chatOutputText;

    // ���ڴ洢�Ի���ʷ
    private List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();
    void Start()
    {
        // ��ʼ��ϵͳ��Ϣ
        messages.Add(new Dictionary<string, string> { { "role", "system" }, { "content", "You are a helpful assistant." } });
    }

    public void OnSendButtonClicked()
    {
        string userMessage = userInputField.text;
        if (string.IsNullOrEmpty(userMessage)) return;

        // ����û���Ϣ���Ի���ʷ
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userMessage } });

        // ���� DeepSeek API
        StartCoroutine(CallDeepSeekAPI());
    }

    private IEnumerator CallDeepSeekAPI()
    {
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
            chatOutputText.text += "\nAI: " + botMessage;

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
}