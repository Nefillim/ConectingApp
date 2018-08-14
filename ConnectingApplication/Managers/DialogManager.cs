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
		public static Dictionary<string, Dialog> ActiveMessageDialogs;
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
				if (!curDialog.Reusable)
				{
					foreach (string ch in curDialog.Participants)
					{
						((NPC)CharacterManager.Characters[ch]).AvailableDialogs[curDialog.DialogueMode].Remove(curDialog);
					}
				}
				return ContinueDialog(0);
			}
		}

		public static List<DialogueNode> ContinueMessengerDialog(int nodeId, string dialogId, string charId)
		{
			Dialog curDialog = ActiveMessageDialogs[dialogId];
			curDialog.currentNode = curDialog.selectableNodes.Find(n => n.Id == nodeId);
			Player player = (Player)CharacterManager.Characters.Values.ToList().Find(p => p is Player);
			if (curDialog.TakeNextNodes(nodeId) != null)
			{
				player.textMessages[dialogId].Enqueue(curDialog.currentNode);
				return curDialog.TakeNextNodes(nodeId);
			}
			else
			{
				ActiveMessageDialogs.Remove(dialogId);
				return null;
			}
		}

		public static void StartDialog(string charId, DialogueMode dialogueMode)
		{
			Dialog newOne = CharacterManager.GetDialog(charId, dialogueMode);
			if (ActiveDialogs.Count() > 0)
			{
				newOne.currentBlock = Core.Dialogues.DialogueBlock.BlockType.body;
			}
			ActiveDialogs.Push(newOne);
		}

		public static List<DialogueNode> ContinueDisscussion(string dialogId)
		{
			return Discussions.Find(d => d.Id == dialogId).TakeNextNodes();
		}
	}
}
