using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using Core.Dialogues;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectingApplication.Managers
{

	public class DialogManager
	{
		private List<Dialog> activeDialogs;
		private Dictionary<string, Dialog> activeMessageDialogs;
		private List<Dialog> discussions;
		private List<Dialog> textMessages;

		public int ActiveDialogsCount { get { return activeDialogs.Count; } }
		public IList<Dialog> ActiveDialogs { get { return activeDialogs.AsReadOnly(); } }

		[Obsolete("Don't use outside the ConnectingApp.")]
		public DialogManager()
		{
			activeDialogs = new List<Dialog>();
			activeMessageDialogs = new Dictionary<string, Dialog>();
		}


		private void SetResultsForNode(DialogueNode dialogueNode)
		{
			dialogueNode.InvokeResult();
			// TODO: сохранить 
		}


		public void AddDiscussion(Dialog dialog)
		{
			discussions.Add(dialog);
		}

		public List<DialogueNode> StartDialog(string charId, DialogueMode dialogueMode)
		{
			Dialog newOne = ConnectingAppManager.CharacterManager.GetDialog(charId, dialogueMode);
			newOne.currentBlock = IsDialogLonely(newOne) ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
			activeDialogs.Add(newOne);
			return ContinueDialog();
		}

		/// <summary>
		/// Чтобы запросить новые ноды 
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public List<DialogueNode> ContinueDialog(DialogueNode dialogueNode = null)
		{
			var curDialog = activeDialogs.Last();
			int nodeId = -1;

			if (dialogueNode != null)
			{
				SetResultsForNode(dialogueNode);
				nodeId = dialogueNode.Id;
			}
			else
			{
				if (curDialog.currentNode != null)
					nodeId = curDialog.currentNode.Id;
			}

			var nodes = curDialog.TakeNextNodes(nodeId);

			if (nodes.Count != 0)
			{
				return nodes;
			}
			else
			{
				activeDialogs.Remove(curDialog);
				if (!curDialog.Reusable)
				{
					foreach (string ch in curDialog.Participants)
					{
						var npc = ConnectingAppManager.CharacterManager.GetNPC(ch);
						npc.AvailableDialogs[curDialog.DialogueMode].Remove(curDialog);
						if (npc.AvailableDialogs[curDialog.DialogueMode].Count == 0)
							npc.ActivateObject(false, curDialog.DialogueMode);
					}
				}
				if (activeDialogs.Count == 0)
					return nodes;
				else return ContinueDialog();
			}
		}

		public void BreakingDialog(string character, string dialogId, DialogueMode dialogueMode, EDialogueResultType breakingType)
		{
			var npc = ConnectingAppManager.CharacterManager.GetNPC(character);
			npc.AvailableDialogs[dialogueMode].Find(s => s.Id.Equals(dialogId)).ActivateResult(breakingType);
		}

		public void BreakingDialog(EDialogueResultType breakingType)
		{
			activeDialogs.Last().ActivateResult(breakingType);
		}

		public List<DialogueNode> ContinueMessengerDialog(int nodeId, string dialogId, string charId, DialogueMode mode)
		{
			Dialog curDialog = activeMessageDialogs[dialogId];
			curDialog.currentNode = curDialog.selectableNodes.Find(n => n.Id == nodeId);
			var player = ConnectingAppManager.CharacterManager.GetPlayer();
			player.AddMessage(charId, curDialog.currentNode, mode);
			if (curDialog.TakeNextNodes(nodeId) != null)
			{
				return curDialog.TakeNextNodes(nodeId);
			}
			else
			{
				activeMessageDialogs.Remove(dialogId);
				return null;
			}
		}

		public List<DialogueNode> ContinueDisscussion(string dialogId)
		{
			return discussions.Find(d => d.Id == dialogId).TakeNextNodes();
		}

		public bool IsDialogLonely(Dialog dialog)
		{
			return activeDialogs.ToList().FindAll(s => s.Participants.First().Equals(dialog.Participants.First())).Count > 0;
		}

		public List<DialogueNode> ContinueDialog(DialogueMode mode, int nodeId = -1, string dialogId = "", string charId = "", DialogueNode dialogueNode = null)
		{
			switch (mode)
			{
				case DialogueMode.call:
					return ContinueDialog(dialogueNode);
				case DialogueMode.email:
				case DialogueMode.sms:
					if (dialogId != "" && charId != "")
					{
						return ContinueMessengerDialog(nodeId, dialogId, charId, mode);
					}
					else { return null; }
				case DialogueMode.videocall:
					return ContinueDialog(dialogueNode);
				case DialogueMode.meet:
					return ContinueDialog(dialogueNode);
				case DialogueMode.dialogueInterface:
					return ContinueDialog(dialogueNode);
				default:
					return ContinueDialog(dialogueNode);
			}
		}

	}
}
