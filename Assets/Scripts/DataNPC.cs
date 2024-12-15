using UnityEngine;

namespace Ou
{
    /// <summary>
    /// NPC資料
    /// </summary>
    [CreateAssetMenu(menuName = "Ou/NPC")]
    public class DataNPC : ScriptableObject
    {
        [Header("NPC AI 要分析的句子")]
        public string[] sentences;
    }
}
