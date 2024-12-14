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
    public class HuggingFaceManager : MonoBehaviour
    {
        private string url = "https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2";
        private string key = "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";

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
            var inputs = new
            {
                source_sentence = prompt,
                sentences = ""
            };

            // �N����ର json �H�ΤW�Ǫ� byte[] �榡
            string json = JsonUtility.ToJson(inputs);
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
