using ConnectingApplication.Characters;
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
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public static class EventResultsManager
    {
        public static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs =
            new Dictionary<ResultFuncsEnum, Action<List<string>>>()
            {
                { ResultFuncsEnum.ActivateDialogue, OnNewAvailableDialog },
                { ResultFuncsEnum.ActivateBusiness, OnChangeBusiness },
            };

        public static void CoreEventsResult(ResultFuncsEnum enumerator, List<string> fields)
        {
            Debug.Log($"ResultType: {enumerator.ToString()}, inputFields: {string.Concat(fields)}");
            ResultFuncs[enumerator].Invoke(fields);
        }

        public static void OnNewAvailableDialog(List<string> dialogues)
        {
            foreach (var d in dialogues)
            {
                var dialogue = CoreController.DialogueManager.GetDialogue(d);
                Dialog dialog = new Dialog(dialogue)
                {
                    selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(d, BlockType.hi, EGetDialogueNodeType.firstNodes)
                };
                CharacterManager.AddDialog(dialog, dialog.selectableNodes.First().Role);
            }
        }

        public static void OnChangeBusiness(List<string> businesses)
        {
            if (businesses.Count > 1)
                throw new FormatException("Игрок не может выбрать больше одного занятия!");
            var business = BusinessManager.GetBusinessInfo(businesses.First());
            DownloadManager.SetNewIteratorPosition();
        }

        public static void OnNewCharacterFact(List<string> facts)
        {
            NPC character = (NPC)CharacterManager.Characters[facts.First()];
            character.CharacterInfo.Add(facts.Last());
        }
    }
}
