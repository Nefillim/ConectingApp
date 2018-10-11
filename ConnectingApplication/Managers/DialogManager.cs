using Assets.ConectingApp.ConnectingApplication.Enums;
using Assets.Scripts;
using ConnectingApplication.Entity;
using Core.Dialogues;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectingApplication.Managers
{

    public class DialogManager
    {
        private readonly List<Dialog> activeDialogs;
        private readonly Dictionary<string, List<Dialog>> activeMessageDialogs;
        private readonly Dictionary<string, List<Dialog>> activeEmailDialogs;
        private readonly List<Dialog> discussions;

        public int ActiveDialogsCount { get { return activeDialogs.Count; } }
        public IList<Dialog> ActiveDialogs { get { return activeDialogs.AsReadOnly(); } }

        [Obsolete("Don't use outside the ConnectingApp.")]
        public DialogManager()
        {
            activeDialogs = new List<Dialog>();
            activeMessageDialogs = new Dictionary<string, List<Dialog>>();
            activeEmailDialogs = new Dictionary<string, List<Dialog>>();
            discussions = new List<Dialog>();
        }


        private List<DialogueNode> ContinueMessengerDialog(string charId, FormatDialogue mode, DialogueNode dialogueNode = null)
        {
            var dialogs = mode == FormatDialogue.sms ? activeMessageDialogs : activeEmailDialogs;
            int nodeId = -1;

            if (!dialogs.ContainsKey(charId) || dialogs[charId].Count == 0)
                return new List<DialogueNode>();
            Dialog curDialog = dialogs[charId].Last();
            if (dialogueNode != null)
            {
                SetResultsForNode(dialogueNode);
                nodeId = dialogueNode.Id;

                var player = ConnectingAppManager.CharacterManager.GetPlayer();
                player.AddMessage(charId, dialogueNode, mode);
            }

            var nextNodes = curDialog.TakeNextNodes(nodeId);

            if (nextNodes.Count == 0)
                dialogs[charId].Remove(curDialog);
            return nextNodes;
        }

        private List<DialogueNode> ContinueDialog(DialogueNode dialogueNode = null)
        {
            var curDialog = activeDialogs.Last();
            int nodeId = -1;

            if (dialogueNode != null)
            {
                //SetResultsForNode(dialogueNode);
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
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.EndOfDialog, new List<string> { curDialog.Format.ToString() });
                activeDialogs.Remove(curDialog);
                if (!curDialog.Reusable)
                {
                    foreach (string ch in curDialog.Participants)
                    {
                        var npc = ConnectingAppManager.CharacterManager.GetNPC(ch);
                        npc.RemoveDialog(curDialog);
                    }
                }
                if (activeDialogs.Count == 0)
                    return nodes;
                else return ContinueDialog();
            }
        }

        private void ActivateResultsForDialog(Dialog dialog, EDialogueResultType eDialogueResultType)
        {
            var resultFlags = dialog.GetResult(eDialogueResultType);
            ConnectingAppManager.FlagManager.SetFlags(resultFlags);
        }


        public void AddDiscussion(Dialog dialog)
        {
            discussions.Add(dialog);
        }

        public void SetResultsForNode(DialogueNode dialogueNode)
        {
            ConnectingAppManager.FlagManager.SetFlags(dialogueNode.Results);
        }

        public List<DialogueNode> StartDialog(string charId, FormatDialogue dialogueMode, string dialogId = "")
        {
            Dialog newOne = ConnectingAppManager.CharacterManager.GetDialog(charId, dialogueMode, dialogId);
            if (newOne != null)
            {
                if (dialogueMode == FormatDialogue.sms || dialogueMode == FormatDialogue.email)
                {
                    newOne.currentBlock = Core.Dialogues.DialogueBlock.BlockType.hi;
                    var dialogs = dialogueMode == FormatDialogue.sms ? activeMessageDialogs : activeEmailDialogs;
                    if (!dialogs.ContainsKey(charId))
                        dialogs.Add(charId, new List<Dialog>());
                    dialogs[charId].Add(newOne);
                    return ContinueMessengerDialog(charId, dialogueMode);
                }
                newOne.currentBlock = IsDialogLonely(newOne) ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
                activeDialogs.Add(newOne);
                return ContinueDialog();
            }
            else return new List<DialogueNode>();
        }

        public void BreakingDialog(string character, string dialogId, FormatDialogue dialogueMode, EDialogueResultType breakingType)
        {
            var npc = ConnectingAppManager.CharacterManager.GetNPC(character);
            var dialog = npc.GetAvailableDialogs(dialogueMode).ToList().Find(s => s.Id.Equals(dialogId));
            ActivateResultsForDialog(dialog, breakingType);
        }

        public void BreakingDialog(EDialogueResultType breakingType)
        {
            ActivateResultsForDialog(activeDialogs.Last(), breakingType);
        }

        public List<DialogueNode> ContinueDisscussion(string dialogId)
        {
            return discussions.Find(d => d.Id == dialogId).TakeNextNodes();
        }

        public bool IsDialogLonely(Dialog dialog)
        {
            return activeDialogs.ToList().FindAll(s => s.Participants.First().Equals(dialog.Participants.First())).Count > 0;
        }

        /// <summary>
        /// Для продолжения уже начатого диалога.
        /// </summary>
        /// <param name="mode">Тип диалога.</param>
        /// <param name="dialogueNode">Последняя выработанная нода диалога.</param>
        /// <param name="charId">Id персонажа с которым идет переписка.</param>
        /// <returns></returns>
        public List<DialogueNode> ContinueDialog(FormatDialogue mode, DialogueNode dialogueNode, string charId = "")
        {
            switch (mode)
            {
                case FormatDialogue.call:
                    return ContinueDialog(dialogueNode);

                case FormatDialogue.email:
                case FormatDialogue.sms:
                    if (charId != "")
                    {
                        return ContinueMessengerDialog(charId, mode, dialogueNode);
                    }
                    else
                    {
                        Debug.LogError("При попытке продолжить диалог не указан charId.");
                        return null;
                    }

                case FormatDialogue.videocall:
                case FormatDialogue.meet:
                case FormatDialogue.dialogueInterface:
                default:
                    return ContinueDialog(dialogueNode);
            }
        }

        public void UpdateDialog(string dialogId)
        {

        }
    }
}
