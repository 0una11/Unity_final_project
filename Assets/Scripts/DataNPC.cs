using UnityEngine;

namespace Ou
{
    /// <summary>
    /// NPC���
    /// </summary>
    [CreateAssetMenu(menuName = "Ou/NPC")]
    public class DataNPC : ScriptableObject
    {
        [Header("NPC AI �n���R���y�l")]
        public string[] sentences;
    }
}
