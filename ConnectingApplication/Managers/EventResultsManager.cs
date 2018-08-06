using ConnectingApplication.Entity;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
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
        public static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs =
            new Dictionary<ResultFuncsEnum, Action<List<string>>>()
            {
                { (ResultFuncsEnum)1, OnNewAvailableDialog },
                { (ResultFuncsEnum)2, OnChangeBusiness },
            };

        public static void Init()
        {

            ResultFuncs.Add((ResultFuncsEnum)1, OnNewAvailableDialog);
        }

        public static void OnNewAvailableDialog(List<string> dialogues)
        {
            foreach (var d in dialogues)
            {
                var dialogue = CoreController.DialogueManager.GetDialogue(d);
                Dialog dialog = new Dialog(dialogue);
                dialog.selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(d, 0, BlockType.hi, EGetDialogueNodeType.actual);
                //TODO: че-нить доделать.
            }
        }

        public static void OnChangeBusiness(List<string> businesses)
        {
            if (businesses.Count > 1)
                throw new FormatException("Игрок не может выбрать больше одного занятия!");
            var business = BusinessManager.GetBusinessInfo(businesses.First());
            DownloadManager.SetNewIteratorPosition();
        }
    }
}
