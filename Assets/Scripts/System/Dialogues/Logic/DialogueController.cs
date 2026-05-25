using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SHYL.Dialogue
{
    //[RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider))]
    public class DialogueController : MonoBehaviour
    {

        public UnityEvent OnFinishEvent;

        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dialogueStack;

        private bool canTalk;

        private bool isTalking;

       // private GameObject uiSign;

        private void Awake()
        {
           // uiSign = transform.GetChild(2).gameObject;
            FillDialogueStack();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = true;
            }
        }
        private void OnTriggerExit(Collider collision)
        {
            canTalk = false;
        }

        private void Update()
        {
           // uiSign.SetActive(canTalk);

            if (canTalk && Input.GetKeyDown(KeyCode.E) && !isTalking)
            {
                StartCoroutine(DialogueRoutine());
            }
        }


        /// <summary>
        /// 构建对话堆栈
        /// </summary>
        private void FillDialogueStack()
        {
            dialogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i > -1; i--)
            {
                dialogueList[i].isDone = false;
                dialogueStack.Push(dialogueList[i]);
            }
        }

        private IEnumerator DialogueRoutine()
        {
            isTalking = true;
            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                //传到UI显示对话
                EventHandler.CallShowDialogueEvent(result);
                //EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                yield return new WaitUntil(() => result.isDone);
                isTalking = false;
            }
            else
            {
                //EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();
                isTalking = false;

                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
            }
        }
        public void Finish()
        {
            TipsMgr.Instance.ShowTaskTips("前往烹饪炉");
            TipsMgr.Instance.ShowSystemTips("获取一本秘籍，请点击右上角查看");
            AddPictrue.Instance.AddImagesFromSet2();
        }
    }
}

