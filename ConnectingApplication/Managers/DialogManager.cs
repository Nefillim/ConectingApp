using ConnectingApplication.Characters;
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


        private void SetResultsForNode(DialogueNode dialogueNode)
        {
            dialogueNode.InvokeResult();
            // TODO: сохранить 
        }

        private List<DialogueNode> ContinueMessengerDialog(string charId, DialogueMode mode, DialogueNode dialogueNode = null)
        {
            var dialogs = mode == DialogueMode.sms ? activeMessageDialogs : activeEmailDialogs;
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
                        npc.RemoveDialog(curDialog.DialogueMode, curDialog);
                        if (npc.GetAvailableDialogs(curDialog.DialogueMode).Count == 0)
                            npc.ActivateObject(false, curDialog.DialogueMode);
                    }
                }
                if (activeDialogs.Count == 0)
                    return nodes;
                else return ContinueDialog();
            }
        }


        public void AddDiscussion(Dialog dialog)
        {
            discussions.Add(dialog);
        }

        public List<DialogueNode> StartDialog(string charId, DialogueMode dialogueMode)
        {
            Dialog newOne = ConnectingAppManager.CharacterManager.GetDialog(charId, dialogueMode);
            if (dialogueMode == DialogueMode.sms || dialogueMode == DialogueMode.email)
            {
                newOne.currentBlock = Core.Dialogues.DialogueBlock.BlockType.body;
                var dialogs = dialogueMode == DialogueMode.sms ? activeMessageDialogs : activeEmailDialogs;
                if (!dialogs.ContainsKey(charId))
                    dialogs.Add(charId, new List<Dialog>());
                dialogs[charId].Add(newOne);
                return ContinueMessengerDialog(charId, dialogueMode);
            }
            newOne.currentBlock = IsDialogLonely(newOne) ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
            activeDialogs.Add(newOne);
            return ContinueDialog();
        }

        public void BreakingDialog(string character, string dialogId, DialogueMode dialogueMode, EDialogueResultType breakingType)
        {
            var npc = ConnectingAppManager.CharacterManager.GetNPC(character);
            npc.GetAvailableDialogs(dialogueMode).ToList().Find(s => s.Id.Equals(dialogId)).ActivateResult(breakingType);
        }

        public void BreakingDialog(EDialogueResultType breakingType)
        {
            activeDialogs.Last().ActivateResult(breakingType);
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
        public List<DialogueNode> ContinueDialog(DialogueMode mode, DialogueNode dialogueNode, string charId = "")
        {
            switch (mode)
            {
                case DialogueMode.call:
                    return ContinueDialog(dialogueNode);

                case DialogueMode.email:
                case DialogueMode.sms:
                    if (charId != "")
                    {
                        return ContinueMessengerDialog(charId, mode, dialogueNode);
                    }
                    else
                    {
                        Debug.LogError("При попытке продолжить диалог не указан charId.");
                        return null;
                    }

                case DialogueMode.videocall:
                case DialogueMode.meet:
                case DialogueMode.dialogueInterface:
                default:
                    return ContinueDialog(dialogueNode);
            }
        }
    }
}
