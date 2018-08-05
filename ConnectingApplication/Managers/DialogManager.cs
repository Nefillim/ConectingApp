using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
	public static class DialogManager
	{
		public static Stack<Dialog> ActiveDialogs;
		public static Stack<Dialog> Discussions;
		public static List<Dialog > TextMessages;

		public static Player player;

		public static List<DialogueNode> ContinueDialog(int nodeId)
		{
			Dialog curDialog = ActiveDialogs.Peek();
			if (curDialog.TakeNextNodes(nodeId) != null)
			{
				return curDialog.TakeNextNodes(nodeId);
			}
			else {
				ActiveDialogs.Pop();
				if (!curDialog.multiusage)
				{
					foreach (Character ch in curDialog.characters)
					{
						ch.AvailableDialogs.Remove(curDialog);
					}
				}
				return ContinueDialog(0);
			}
		}
			
		public static void StartDialog(string dialogId)
		{
			newOne.id = dialogId;
			if (ActiveDialogs.Count() > 0)
			{
				newOne.currentBlock = Core.Dialogues.DialogueBlock.BlockType.body;
			}
			newOne.selectableNodes = Core.CoreController.DialogueManager.GetNodesForDialogue(dialogId, 0, newOne.currentBlock ,Core.Dialogues.EGetDialogueNodeType.actual);
			ActiveDialogs.Push(newOne);
		}


	}
}
