using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Ou
{
    /// <summary>
    /// �ҫ��޲z��
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        [Header("API �t�m")]
        [SerializeField] private string apiUrl = "https://g.ubitus.ai/v1/chat/completions";
        [SerializeField] private string apiKey = "d4pHv5n2G3q2vkVMPen3vFMfUJic4huKiQbvMmGLWUVIr/ptUuGnsCx/zVdYmVtdrGPO9//2h8Fbp6HkSQ0/oA==";

        [Header("����]�w")]
        [SerializeField] private string role = "�A�O�@���Y�ª����թx";

        [Header("UI ����")]
        [SerializeField] private TMP_InputField inputField; // ��J���
        [SerializeField] private TMP_Text outputText;       // ��X��r���
        [SerializeField] private TMP_Text statusText;      // ���A���ܡ]�p���ШD��...���^
        [SerializeField] private GameObject introductionPanel; // �I�����Э��O
        [SerializeField] private GameObject mainUI;        // �C���D�ɭ�

        [Header("NPC ���")]
        [SerializeField] private NPCController npcController; // �s�� NPC ���

        private void Start()
        {
            // ��l�� UI ���A
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

            // ���m���A����
            UpdateStatus("�п�J���D�}�l���ʡI");
        }
        private void Awake()
        {
            // �j�w��J��쪺�����s��ƥ�
            if (inputField != null)
            {
                inputField.onEndEdit.AddListener(PlayerInput);
            }
        }

        private void PlayerInput(string input)
        {
            // �ˬd��J�O�_����
            if (string.IsNullOrWhiteSpace(input))
            {
                UpdateStatus("��J���ର�šI");
                return;
            }

            // �o�e�ШD
            UpdateStatus("�ШD��...");
            StartCoroutine(GetResult(input));
        }

        private IEnumerator GetResult(string prompt)
        {
            // �c�ؽШD�ƾ�
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

            // �N�ƾ��ഫ�� JSON
            string json = JsonConvert.SerializeObject(requestData);

            // �t�m UnityWebRequest
            byte[] postData = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // �o�e�ШD
            yield return request.SendWebRequest();

            // �B�z�^��
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Response: {responseText}");

                try
                {
                    // �ϥ� JSON.Net �ѪR�^��
                    var response = JsonConvert.DeserializeObject<ResponseData>(responseText);
                    if (response?.choices?.Length > 0)
                    {
                        string content = response.choices[0].message.content;
                        outputText.text = content;
                        UpdateStatus("�ШD�����I");
                        TriggerAnimation(content); // �ھڦ^�����eĲ�o�ʵe
                    }
                    else
                    {
                        outputText.text = "���A����^�����G�L�k�ѪR�I";
                        UpdateStatus("�ѪR����");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON Parsing Error: {e.Message}");
                    outputText.text = "�L�k�ѪR���A���^���I";
                    UpdateStatus("�ѪR���~");
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler.text}");
                outputText.text = $"�ШD���ѡG{request.error}";
                UpdateStatus("�ШD����");
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
            if (npcController == null)
            {
                Debug.LogError("NPC Controller �����T�]�m�I");
                return;
            }

            Debug.Log($"Processing response for animation trigger: {response}");

            // �ϥι�����Ӻ޲zĲ�o����
            var animationMap = new[]
            {
                new { Keywords = new[] { "����", "�ּ�" }, AnimationIndex = 1 },
                new { Keywords = new[] { "�ͮ�", "����" }, AnimationIndex = 4 },
                new { Keywords = new[] { "���", "�x�b" }, AnimationIndex = 2 },
                new { Keywords = new[] { "�I�Y", "�z��" }, AnimationIndex = 3 }
            };

            foreach (var map in animationMap)
            {
                foreach (var keyword in map.Keywords)
                {
                    if (response.Contains(keyword))
                    {
                        npcController.PlayAnimation(map.AnimationIndex);
                        Debug.Log($"Triggered animation index: {map.AnimationIndex} for keyword: {keyword}");
                        return;
                    }
                }
            }

            // �p�G�S���ǰt������A�h�����q�{�ʵe
            npcController.PlayAnimation(0);
            Debug.Log("Default animation triggered.");
        }
    }

    /// <summary>
    /// �Ω�ѪR�^������
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




