using ConnectingApplication.Entity;
using Core.Parser.Results.ResultFunc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
	public static class EventResultsManager
	{
		public static Dictionary<ResultFuncsEnum, Action<string>> ResultFuncs =
			new Dictionary<ResultFuncsEnum, Action<string>>();

		public static void Init()
		{

			ResultFuncs.Add((ResultFuncsEnum)1, OnNewAvailableDialog);
		}

		public static void OnNewAvailableDialog(string dialogId)
		{
			Dialog d = new Dialog(Core.CoreController.DialogueManager.GetDialogue(dialogId));
			d.id = dialogId;
			d.selectableNodes = Core.CoreController.DialogueManager.GetNodesForDialogue(dialogId, 0,  (Core.Dialogues.DialogueBlock.BlockType)(d.currentBlock) , Core.Dialogues.EGetDialogueNodeType.actual);
		}


	}
}
