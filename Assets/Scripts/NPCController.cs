using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ou
{
    /// <summary>
    /// NPC ���
    /// </summary>
    public class NPCController : MonoBehaviour
    {
        //�ǦC�e���:�N�p�H�ܼ���ܦbUnity�ݩʭ��O
        [SerializeField, Header("NPC ���")]
        private DataNPC dateNPC;
        [SerializeField, Header("�ʵe�Ѽ�")]
        private string[] paramaters =
        {
            "Ĳ�o��","Ĳ�o�]", "Ĳ�o���Y","Ĳ�o�I�Y","Ĳ�o�Ĥ�"
        };

        //Unity���ʵe����t��
        private Animator ani;

        public DataNPC data => dateNPC;

        //����ƥ�:����C���ɷ|����@��
        private void Awake()
        {
            //��oNPC���W���ʵe���
            ani = GetComponent<Animator>();
        }

        public void PlayAnimation(int index)
        {
            ani.SetTrigger(paramaters[index]);
        }
    }
}

