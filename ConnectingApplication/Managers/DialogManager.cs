﻿using Assets.ConectingApp.ConnectingApplication.Enums;
using Assets.Scripts;
using Assets.Scripts.Helpers;
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
        public Dialog ActualDialog { get { return (activeDialogs.Count > 0) ? activeDialogs.Last() : null; } }

        [Obsolete("Don't use outside the ConnectingApp.")]
        public DialogManager()
        {
            activeDialogs = new List<Dialog>();
            ShowCoreAndConnectingAppEntities.Instance.ActiveDialogues = activeDialogs;

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
                //SetResultsForNode(dialogueNode);
                nodeId = dialogueNode.Id;

                var player = ConnectingAppManager.CharacterManager.GetPlayer();
                player.AddMessage(charId, dialogueNode, mode == FormatDialogue.sms ? Characters.Player.MessageType.Sms : Characters.Player.MessageType.Email, curDialog.Id);
            }

            var nextNodes = curDialog.TakeNextNodes(nodeId);

            if (nextNodes.Count == 0)
            {
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.EndOfDialog, new List<string> { curDialog.Id, curDialog.Format.ToString() });
                dialogs[charId].Remove(curDialog);
            }
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
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.EndOfDialog, new List<string> { curDialog.Id, curDialog.Format.ToString() });
                activeDialogs.Remove(curDialog);
                if (curDialog.currentBlock == Core.Dialogues.DialogueBlock.BlockType.bye)
                {
                    var npcId = ConnectingAppManager.CharacterManager.GetNPC(curDialog.Participants.First()).Id;
                    activeDialogs.RemoveAll(d => d.Participants.Contains(npcId));
                }
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

        private void ActivateResultsForDialogBreak(Dialog dialog, EBreakingResultType eDialogueResultType)
        {
            var resultFlags = dialog.GetBreakingResults(eDialogueResultType);
            ConnectingAppManager.FlagManager.SetFlags(resultFlags);
        }


        public void SetResultsForDialog(Dialog dialog)
        {
            var resultFlags = dialog.GetDialogueResults();
            ConnectingAppManager.FlagManager.SetFlags(resultFlags);
        }

        public Dialog GetDiscussion(string id)
        {
            return discussions.Find(s => s.Id.Equals(id));
        }

        public void AddDiscussion(Dialog d)
        {
            discussions.Add(d);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.StartDialogue, new List<string>() { d.Participants.First(),
                                                                                                  d.Id, ((int)d.Format).ToString(),
                                                                                                  ((int)d.CharacterOfDialogue).ToString() });
        }

        public void SetResultsForNode(DialogueNode dialogueNode)
        {
            ConnectingAppManager.FlagManager.SetFlags(dialogueNode.Results);
        }

        public List<DialogueNode> StartDialog(string charId, FormatDialogue dialogueMode, string dialogId)
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
                activeDialogs.Add(newOne);
                newOne.currentBlock = !IsLonelyDialog(newOne) ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
                return ContinueDialog();
            }
            else return new List<DialogueNode>();
        }

        public List<DialogueNode> StartDiscussion(string dialogueId)
        {
            return discussions.Find(d => d.Id == dialogueId).TakeNextNodes(-1);
        }

        public void BreakingDialog(string character, string dialogId, FormatDialogue dialogueMode, EBreakingResultType breakingType)
        {
            var npc = ConnectingAppManager.CharacterManager.GetNPC(character);
            var dialog = npc.GetAvailableDialogs(dialogueMode).ToList().Find(s => s.Id.Equals(dialogId));
            if (dialog != null)
                ActivateResultsForDialogBreak(dialog, breakingType);
        }  

        public void BreakingDialog(EBreakingResultType breakingType)
        {
            if (ActualDialog != null)
                ActivateResultsForDialogBreak(ActualDialog, breakingType);
        }

        public List<DialogueNode> ContinueDisscussion(string dialogId, DialogueNode dialogueNode = null)
        {
            int nodeId = -1;

            if (discussions.Count == 0)
                return new List<DialogueNode>();

            Dialog curDialog = discussions.Find(d => d.Id == dialogId);
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

            var nextNodes = curDialog.TakeNextNodes(nodeId);

            if (nextNodes.Count == 0)
            {
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.EndOfDialog, new List<string> { curDialog.Id, curDialog.Format.ToString() });
                discussions.Remove(curDialog);
            }
            return nextNodes;
        }

        public bool IsLonelyDialog(Dialog dialog)
        {
            return activeDialogs.ToList().FindAll(s => s.Participants.First().Equals(dialog.Participants.First())).Count <= 1;
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

        public bool CheckActualDialog(Predicate<Dialog> predicate)
        {
            return ActualDialog != null && predicate.Invoke(ActualDialog);
        }
    }
}
