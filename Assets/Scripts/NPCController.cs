using UnityEngine;
using static NewBehaviourScript;

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

        //Unity���ʵe����t��
        private Animator ani;

        //����ƥ�:����C���ɷ|����@��
        private void Awake()
        {
            //��oNPC���W���ʵe���
            ani = GetComponent<Animator>();
        }
    }
}

