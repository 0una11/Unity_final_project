using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ou
{
    /// <summary>
    /// �ҫ��޲z��
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        private string url = "https://g.ubitus.ai/v1/chat/completions";
        private string key = "d4pHv5n2G3q2vkVMPen3vFMfUJic4huKiQbvMmGLWUVIr/ptUuGnsCx/zVdYmVtdrGPO9//2h8Fbp6HkSQ0/oA==";

        private TMP_InputField inputField; // ��J���
        public TMP_Text outputText;       // ��X��r���
        private string prompt;            // �Τ��J������
        private string role = "�A�O�@���Y�ª����թx";

        private void Awake()
        {
            // ��l�ƿ�J�P��X���
            inputField = GameObject.Find("��J���").GetComponent<TMP_InputField>();
            outputText = GameObject.Find("��X���").GetComponent<TMP_Text>();

            // �j�w��J��쪺�����s��ƥ�
            inputField.onEndEdit.AddListener(PlayerInput);
        }

        private void PlayerInput(string input)
        {
            prompt = input;
            StartCoroutine(GetResult());
        }

        private IEnumerator GetResult()
        {
            // �c�ؽШD�� JSON �ƾ�
            var data = new
            {
                model = "llama-3.1-70b",
                messages = new[]
                {
                    new
                    {
                        name = "user",
                        content = prompt,
                        role = this.role
                    }
                },
                stop = new string[] { "<|eot_id|>", "<|end_of_text|>" },
                frequency_penalty = 0,
                max_tokens = 100,
                temperature = 0.2,
                top_p = 0.5,
                top_k = 20,
                stream = false
            };

            // �N�ƾ��ഫ�� JSON
            string json = JsonUtility.ToJson(data);
            Debug.Log($"Request JSON: {json}"); // ���L�ШD JSON �H�K�ո�

            // �t�m UnityWebRequest
            byte[] postData = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + key);

            // �o�e�ШD
            yield return request.SendWebRequest();

            // �B�z�^��
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Response: {responseText}");

                // ���ոѪR�^���ç�s UI
                try
                {
                    // �ϥ� JsonUtility �Ψ�L JSON �ѪR�w
                    var response = JsonUtility.FromJson<ResponseData>(responseText);
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        outputText.text = response.choices[0].message.content;
                    }
                    else
                    {
                        outputText.text = "���A����^�����G�L�k�ѪR�I";
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON Parsing Error: {e.Message}");
                    outputText.text = "�L�k�ѪR���A���^���I";
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler.text}");
                outputText.text = $"�ШD���ѡG{request.error}";
            }
        }
    }

    /// <summary>
    /// �Ω�ѪR�^������
    /// </summary>
    [System.Serializable]
    public class ResponseData
    {
        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public Message message;

            [System.Serializable]
            public class Message
            {
                public string content;
            }
        }
    }
}


