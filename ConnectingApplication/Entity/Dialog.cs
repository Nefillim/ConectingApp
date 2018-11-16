using ConnectingApplication.Characters;
using ConnectingApplication.Entity.Characters;
using ConnectingApplication.Managers;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConnectingApplication.Entity
{
    [Serializable]
    public class Dialog : Dialogue
    {
        private List<DialogueNode> selectableNodes;

		public struct MessNode {
			public DialogueNode node;
			public int date;
		}

        public DialogueNode currentNode;
        public Core.Dialogues.DialogueBlock.BlockType currentBlock;


        public Dialog(Dialogue dialgoue) : base(dialgoue)
        {
            selectableNodes = new List<DialogueNode>();
        }


        private List<DialogueNode> MakeDecision(List<DialogueNode> newNodes)
        {
            switch (currentBlock)
            {
                case Core.Dialogues.DialogueBlock.BlockType.hi:
                    currentBlock++;
                    return TakeNextNodes(-1);

                case Core.Dialogues.DialogueBlock.BlockType.body:
                    if (ConnectingAppManager.DialogManager.IsLonelyDialog(this))
                        currentBlock++;
                    else
                        currentBlock = Core.Dialogues.DialogueBlock.BlockType.next;
                    return TakeNextNodes(-1);

                case Core.Dialogues.DialogueBlock.BlockType.bye:
					break;
				case Core.Dialogues.DialogueBlock.BlockType.next:
					List<DialogueNode> dialogueNodes =  CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.firstNodes);
					if (dialogueNodes.Count == 0)
					{
						currentBlock = Core.Dialogues.DialogueBlock.BlockType.bye;
                        return TakeNextNodes(-1);
					}
					break;

                default:
                    break;
            }
            return newNodes;
        }


        public List<DialogueNode> TakeNextNodes()
        {
            List<DialogueNode> newNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.next, currentNode.Id);
            if (newNodes.Count > 0)
            {
                selectableNodes = newNodes;
                return newNodes;
            }
            else
            {
                return MakeDecision(newNodes);
            }
        }

        /// <summary>
        /// Если nodeId==-1, то запрашиваем первые ноды для диалога.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<DialogueNode> TakeNextNodes(int nodeId)
        {
            if (nodeId == -1)
            {
                // Существует проблема, если сюда попадают из блока hi, и блок body пуст(даже если по ошибке), а блок next или bye не пусты, они все равно не обработаются.
                selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.firstNodes);
                if (selectableNodes.Count > 0)
                    return selectableNodes;
                else return MakeDecision(selectableNodes);
            }
            else
            {
                currentNode = selectableNodes.Find(n => n.Id == nodeId);
                if (currentNode == null)
                {
                    selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.next, nodeId);

                    // Эта ситуация возникнет только если программа логически перешла в другой блок диалога, 
                    // но последняя отработавшая нода принадлежит предыдущему блоку(логически предыдущему, а не по порядку).
                    if (selectableNodes.Count == 0)
                        selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.firstNodes, -1);

                    return selectableNodes;
                }
                else return TakeNextNodes();
            }
        }
    }
}
