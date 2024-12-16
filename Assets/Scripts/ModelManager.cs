using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // 使用 JSON.Net 庫進行 JSON 解析

namespace Ou
{
    /// <summary>
    /// 模型管理器：處理與語言模型 API 的互動
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        [Header("API 配置")]
        [SerializeField] private string apiUrl = "https://g.ubitus.ai/v1/chat/completions";
        [SerializeField] private string apiKey = "d4pHv5n2G3q2vkVMPen3vFMfUJic4huKiQbvMmGLWUVIr/ptUuGnsCx/zVdYmVtdrGPO9//2h8Fbp6HkSQ0/oA==";

        [Header("角色設定")]
        [SerializeField] private string role = "你是一個嚴肅的面試官";

        [Header("UI 元件")]
        [SerializeField] private TMP_InputField inputField; // 輸入欄位
        [SerializeField] private TMP_Text outputText;       // 輸出文字欄位
        [SerializeField] private TMP_Text statusText;      // 狀態提示（如“請求中...”）

        private void Awake()
        {
            // 綁定輸入欄位的結束編輯事件
            if (inputField != null)
            {
                inputField.onEndEdit.AddListener(PlayerInput);
            }
        }

        private void PlayerInput(string input)
        {
            // 檢查輸入是否為空
            if (string.IsNullOrWhiteSpace(input))
            {
                UpdateStatus("輸入不能為空！");
                return;
            }

            // 發送請求
            UpdateStatus("請求中...");
            StartCoroutine(GetResult(input));
        }

        private IEnumerator GetResult(string prompt)
        {
            // 構建請求數據
            var requestData = new
            {
                model = "llama-3.1-70b",
                messages = new[]
                {
                    new { name = "user", content = prompt, role = role }
                },
                stop = new[] { "<|eot_id|>", "<|end_of_text|>" },
                frequency_penalty = 0,
                max_tokens = 100,
                temperature = 0.2f,
                top_p = 0.5f,
                top_k = 20,
                stream = false
            };

            // 將數據轉換為 JSON
            string json = JsonConvert.SerializeObject(requestData);

            // 配置 UnityWebRequest
            byte[] postData = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // 發送請求
            yield return request.SendWebRequest();

            // 處理回應
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Response: {responseText}");

                try
                {
                    // 使用 JSON.Net 解析回應
                    var response = JsonConvert.DeserializeObject<ResponseData>(responseText);
                    if (response?.choices?.Length > 0)
                    {
                        string content = response.choices[0].message.content;
                        outputText.text = content;
                        UpdateStatus("請求完成！");
                    }
                    else
                    {
                        outputText.text = "伺服器返回的結果無法解析！";
                        UpdateStatus("解析失敗");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON Parsing Error: {e.Message}");
                    outputText.text = "無法解析伺服器回應！";
                    UpdateStatus("解析錯誤");
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler.text}");
                outputText.text = $"請求失敗：{request.error}";
                UpdateStatus("請求失敗");
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
    }

    /// <summary>
    /// 用於解析回應的類
    /// </summary>
    public class ResponseData
    {
        public Choice[] choices;

        public class Choice
        {
            public Message message;

            public class Message
            {
                public string content;
            }
        }
    }
}



