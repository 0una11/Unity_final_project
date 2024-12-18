using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Ou
{
    /// <summary>
    /// 模型管理器
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
        [SerializeField] private GameObject introductionPanel; // 背景介紹面板
        [SerializeField] private GameObject mainUI;        // 遊戲主界面

        [Header("NPC 控制器")]
        [SerializeField] private NPCController npc; // 連接 NPC 控制器

        private void Start()
        {
            // 初始化 UI 狀態
            ShowIntroduction();
        }

        private void ShowIntroduction()
        {
            if (introductionPanel != null)
            {
                introductionPanel.SetActive(true);
            }

            if (mainUI != null)
            {
                mainUI.SetActive(false);
            }
        }

        public void StartGame()
        {
            if (introductionPanel != null)
            {
                introductionPanel.SetActive(false);
            }

            if (mainUI != null)
            {
                mainUI.SetActive(true);
            }

            // 重置狀態提示
            UpdateStatus("請輸入問題開始互動！");
        }
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
                        TriggerAnimation(content); // 根據回應內容觸發動畫
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

        private void TriggerAnimation(string response)
        {
            if (npc == null)
            {
                Debug.LogError("NPC Controller 未正確設置！");
                return;
            }

            Debug.Log($"Processing response for animation trigger: {response}");

            // 使用對應表來管理觸發條件
            var animationMap = new[]
            {
                new { Keywords = new[] { "高興", "快樂" }, AnimationIndex = 1 },
                new { Keywords = new[] { "生氣", "憤怒" }, AnimationIndex = 4 },
                new { Keywords = new[] { "思考", "困惑" }, AnimationIndex = 2 },
                new { Keywords = new[] { "點頭", "理解" }, AnimationIndex = 3 }
            };

            foreach (var map in animationMap)
            {
                foreach (var keyword in map.Keywords)
                {
                    if (response.Contains(keyword))
                    {
                        npc.PlayAnimation(map.AnimationIndex);
                        Debug.Log($"Triggered animation index: {map.AnimationIndex} for keyword: {keyword}");
                        return;
                    }
                }
            }

            // 如果沒有匹配的條件，則執行默認動畫
            npc.PlayAnimation(0);
            Debug.Log("Default animation triggered.");
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




