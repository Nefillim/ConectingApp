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

namespace ConnectingApplication.Entity
{
	public class Dialog : Core.Dialogues.Dialogue
	{
		public List<DialogueNode> selectableNodes;
		public List<string> characters;
		public DialogueNode currentNode;
		public bool multiusage;
		public Core.Dialogues.DialogueBlock.BlockType currentBlock;
		public string id;


		public Dialog(Dialogue dialgoue)
		{
			id = dialgoue.Id;
			characters.AddRange(dialgoue.Participants);
			selectableNodes = new List<DialogueNode>();
			multiusage = dialgoue.Reusable;

		}

		public List<DialogueNode> TakeNextNodes()
		{
			List<DialogueNode> newNodes = new List<DialogueNode>();
			if (CoreController.DialogueManager.GetNodesForDialogue
				(id.ToString(), currentNode.Id, currentBlock, EGetDialogueNodeType.next) != null)
			{
				newNodes = CoreController.DialogueManager.GetNodesForDialogue(id.ToString(), currentNode.Id, currentBlock, Core.Dialogues.EGetDialogueNodeType.next);
				return newNodes;
			}
			else
			{
				switch (currentBlock)
				{
					case Core.Dialogues.DialogueBlock.BlockType.hi:
						currentBlock++;
						break;
					case Core.Dialogues.DialogueBlock.BlockType.body:
						if (DialogManager.ActiveDialogs.Count == 1)
							currentBlock++;
						else
							currentBlock = Core.Dialogues.DialogueBlock.BlockType.next;
						break;
					case Core.Dialogues.DialogueBlock.BlockType.next:

						break;
					default:
						break;
				}
				return null;
			}
		}

		public List<DialogueNode> TakeNextNodes(int nodeId)
		{
			List<DialogueNode> newNodes = new List<DialogueNode>();
			if (CoreController.DialogueManager.GetNodesForDialogue
				(id.ToString(), nodeId, currentBlock ,EGetDialogueNodeType.next) != null)
			{
				newNodes = CoreController.DialogueManager.GetNodesForDialogue(id.ToString(), nodeId, currentBlock,Core.Dialogues.EGetDialogueNodeType.next);
				return newNodes;
			}
			else {
				switch (currentBlock)	
				{
					case Core.Dialogues.DialogueBlock.BlockType.hi:
						currentBlock++;
						break;
					case Core.Dialogues.DialogueBlock.BlockType.body:
						if (DialogManager.ActiveDialogs.Count == 1)
							currentBlock++;
						else
							currentBlock = Core.Dialogues.DialogueBlock.BlockType.next;
						break;
					case Core.Dialogues.DialogueBlock.BlockType.next:

						break;
					default:
						break;
				}
				return null;
			}
		}
	}
}
