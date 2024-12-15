using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ou
{
    /// <summary>
    /// 模型管理器
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        private string url = "https://g.ubitus.ai/v1/chat/completions";
        private string key = "d4pHv5n2G3q2vkVMPen3vFMfUJic4huKiQbvMmGLWUVIr/ptUuGnsCx/zVdYmVtdrGPO9//2h8Fbp6HkSQ0/oA==";

        private TMP_InputField inputField; // 輸入欄位
        public TMP_Text outputText;       // 輸出文字欄位
        private string prompt;            // 用戶輸入的提示
        private string role = "你是一個嚴肅的面試官";

        private void Awake()
        {
            // 初始化輸入與輸出欄位
            inputField = GameObject.Find("輸入欄位").GetComponent<TMP_InputField>();
            outputText = GameObject.Find("輸出欄位").GetComponent<TMP_Text>();

            // 綁定輸入欄位的結束編輯事件
            inputField.onEndEdit.AddListener(PlayerInput);
        }

        private void PlayerInput(string input)
        {
            prompt = input;
            StartCoroutine(GetResult());
        }

        private IEnumerator GetResult()
        {
            // 構建請求的 JSON 數據
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

            // 將數據轉換為 JSON
            string json = JsonUtility.ToJson(data);
            Debug.Log($"Request JSON: {json}"); // 打印請求 JSON 以便調試

            // 配置 UnityWebRequest
            byte[] postData = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + key);

            // 發送請求
            yield return request.SendWebRequest();

            // 處理回應
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Response: {responseText}");

                // 嘗試解析回應並更新 UI
                try
                {
                    // 使用 JsonUtility 或其他 JSON 解析庫
                    var response = JsonUtility.FromJson<ResponseData>(responseText);
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        outputText.text = response.choices[0].message.content;
                    }
                    else
                    {
                        outputText.text = "伺服器返回的結果無法解析！";
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON Parsing Error: {e.Message}");
                    outputText.text = "無法解析伺服器回應！";
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler.text}");
                outputText.text = $"請求失敗：{request.error}";
            }
        }
    }

    /// <summary>
    /// 用於解析回應的類
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


