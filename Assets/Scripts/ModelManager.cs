using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ou
{
    ///<summary>
    ///�ҫ��޲z��
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        private string url = "https://g.ubitus.ai/v1/chat/completions";
        private string key = "d4pHv5n2G3q2vkVMPen3vFMfUJic4huKiQbvMmGLWUVIr/ptUuGnsCx/zVdYmVtdrGPO9//2h8Fbp6HkSQ0/oA==";

        private TMP_InputField inputField;
        private string prompt;
        private string role = "�A�O�@���Y�ª����թx";

        // ����ƥ�:�C�������|����@��
        private void Awake()
        {
            // �M������W�W�٬� ��J��� ������æs��� inputField �ܼƤ�
            inputField = GameObject.Find("��J���").GetComponent<TMP_InputField>();
            // ���a�����s���J���ɷ|���� PlayerInput ��k
            inputField.onEndEdit.AddListener(PlayerInput);
        }

        private void PlayerInput(string input)
        {
            print($"<color=#3f3>{input}</color>");
            prompt = input;
            // �Ұʰ��P�{�ǡA��o���G
            StartCoroutine(GetResult());
        }

        private IEnumerator GetResult()
        {
            var data = new
            {
                model = "llama-3.1-70b",
                messages = new
                {
                    name = "user",
                    content = prompt,
                    role = this.role
                },
                stop = new string[] { "<|eot_id|>", "<|end_of_text|>" },
                frequency_penalty = 0,
                max_tokens = 2000,
                temperature = 0.2,
                top_p = 0.5,
                top_k = 20,
                stream = false

            };

            // �N����ର json �H�ΤW�Ǫ� byte[] �榡
            string json = JsonUtility.ToJson(data);
            byte[] postData = Encoding.UTF8.GetBytes(json);
            // �z�L POST �N��ƶǻ���ҫ����A���ó]�w���D
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer" + key);

            yield return request.SendWebRequest();

            print(request.result);
        }
    }
}

