using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Ou
{
    ///<summary>
    ///模型管理器
    /// </summary>
    public class HuggingFaceManager : MonoBehaviour
    {
        private string url = "https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2";
        private string key = "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";

        private TMP_InputField inputField;
        private string prompt;
        private string role = "你是一個嚴肅的面試官";
        private string[] npcSentences;

        [SerializeField, Header("NPC 物件")]
        private NPCController npc;


        // 喚醒事件:遊戲撥放後會執行一次
        private void Awake()
        {
            // 尋找場景上名稱為 輸入欄位 的物件並存放到 inputField 變數內
            inputField = GameObject.Find("輸入欄位").GetComponent<TMP_InputField>();
            // 當玩家結束編輯輸入欄位時會執行 PlayerInput 方法
            inputField.onEndEdit.AddListener(PlayerInput);
            // 獲得npc要分析的語句
            npcSentences = npc.data.sentences;
        }

        private void PlayerInput(string input)
        {
            print($"<color=#3f3>{input}</color>");
            prompt = input;
            // 啟動偕同程序，獲得結果
            StartCoroutine(GetResult());
        }

        private IEnumerator GetResult()
        {
            var inputs = new
            {
                source_sentence = prompt,
                sentences = npcSentences
            };

            // 將資料轉為 json 以及上傳的 byte[] 格式
            string json = JsonConvert.SerializeObject(inputs);
            byte[] postData = Encoding.UTF8.GetBytes(json);
            // 透過 POST 將資料傳遞到模型伺服器並設定標題
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + key);

            yield return request.SendWebRequest();


            if (request.result != UnityWebRequest.Result.Success)
            {
                print($"<color=#f3d>要求失敗 : {request.error}</color>");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<List<string>>(responseText);
                print($"<color=#3f3>分數:{responseText}</color>");

                if (response != null && response.Count > 0)
                {
                    int best = response.Select((value, index) => new
                    {
                        Value = value,
                        Index = index
                    }).OrderByDescending(x => x.Value).First().Index;

                    print($"<color=#3f3>最佳答案:{npcSentences[best]}</color>");

                    npc.PlayAnimation(best);
                }
            }
            print(request.result);
            print(request.error);

        }
    }
}
