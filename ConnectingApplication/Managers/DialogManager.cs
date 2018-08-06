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
		public static List<Dialog> Discussions;
		public static List<Dialog> TextMessages;

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
					foreach (string ch in curDialog.characters)
					{
						CharacterManager.Characters.Find(c => c.id == ch).AvailableDialogs.Remove(curDialog);
					}
				}
				return ContinueDialog(0);
			}
		}
			
		public static void StartDialog(string dialogId, string charId)
		{
			Dialog newOne = CharacterManager.GetDialog(charId, dialogId);
			if (ActiveDialogs.Count() > 0)
			{
				newOne.currentBlock = Core.Dialogues.DialogueBlock.BlockType.body;
			}
			newOne.selectableNodes = Core.CoreController.DialogueManager.GetNodesForDialogue(dialogId, 0, newOne.currentBlock ,Core.Dialogues.EGetDialogueNodeType.actual);
			ActiveDialogs.Push(newOne);
		}

		public static List<DialogueNode> ContinueDisscussion(string dialogId)
		{
			return Discussions.Find(d => d.id == dialogId).TakeNextNodes();
		}


	}
}
