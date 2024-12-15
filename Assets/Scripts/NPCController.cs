using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ou
{
    /// <summary>
    /// NPC 控制器
    /// </summary>
    public class NPCController : MonoBehaviour
    {
        //序列畫欄位:將私人變數顯示在Unity屬性面板
        [SerializeField, Header("NPC 資料")]
        private DataNPC dateNPC;
        [SerializeField, Header("動畫參數")]
        private string[] paramaters =
        {
            "觸發看","觸發跑", "觸發抬頭","觸發點頭","觸發融化"
        };

        //Unity的動畫控制系統
        private Animator ani;

        public DataNPC data => dateNPC;

        //喚醒事件:撥放遊戲時會執行一次
        private void Awake()
        {
            //獲得NPC身上的動畫控制器
            ani = GetComponent<Animator>();
        }

        public void PlayAnimation(int index)
        {
            ani.SetTrigger(paramaters[index]);
        }
    }
}

