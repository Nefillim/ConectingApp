﻿using ConnectingApplication.Characters;
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
        public List<Dialog> ActiveDialogs;
        public Dictionary<string, Dialog> ActiveMessageDialogs;
        public List<Dialog> Discussions;
        public List<Dialog> TextMessages;

        public Player player;


        [Obsolete("Don't use outside the ConnectingApp.")]
        public DialogManager()
        {
            ActiveDialogs = new List<Dialog>();
            ActiveMessageDialogs = new Dictionary<string, Dialog>();
        }


        public void SetResultsForNode(DialogueNode dialogueNode)
        {
            dialogueNode.InvokeResult();
            // TODO: сохранить 
        }

        public List<DialogueNode> StartDialog(string charId, DialogueMode dialogueMode)
        {
            Dialog newOne = ConnectingAppManager.CharacterManager.GetDialog(charId, dialogueMode);
            newOne.currentBlock = IsDialogLonely(newOne) ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
            ActiveDialogs.Add(newOne);
            return ContinueDialog();
        }

        /// <summary>
        /// Чтобы запросить новые ноды 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<DialogueNode> ContinueDialog(DialogueNode dialogueNode = null)
        {
            var curDialog = ActiveDialogs.Last();
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
                ActiveDialogs.Remove(curDialog);
                if (!curDialog.Reusable)
                {
                    foreach (string ch in curDialog.Participants)
                    {
                        NPC npc = ((NPC)ConnectingAppManager.CharacterManager.Characters[ch]);
                        npc.AvailableDialogs[curDialog.DialogueMode].Remove(curDialog);
                        if (npc.AvailableDialogs[curDialog.DialogueMode].Count == 0)
                            npc.ActivateObject(false, curDialog.DialogueMode);
                    }
                }
                if (ActiveDialogs.Count == 0)
                    return nodes;
                else return ContinueDialog();
            }
        }

        public void BreakingDialog(string character, string dialogId, DialogueMode dialogueMode, EDialogueResultType breakingType)
        {
            var npc = ((NPC)ConnectingAppManager.CharacterManager.Characters[character]);
            npc.AvailableDialogs[dialogueMode].Find(s => s.Id.Equals(dialogId)).ActivateResult(breakingType);
        }

        public void BreakingDialog(EDialogueResultType breakingType)
        {
            ActiveDialogs.Last().ActivateResult(breakingType);
        }

        public List<DialogueNode> ContinueMessengerDialog(int nodeId, string dialogId, string charId)
        {
            Dialog curDialog = ActiveMessageDialogs[dialogId];
            curDialog.currentNode = curDialog.selectableNodes.Find(n => n.Id == nodeId);
            Player player = (Player)ConnectingAppManager.CharacterManager.Characters.Values.ToList().Find(p => p is Player);
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

        public List<DialogueNode> ContinueDisscussion(string dialogId)
        {
            return Discussions.Find(d => d.Id == dialogId).TakeNextNodes();
        }

        public bool IsDialogLonely(Dialog dialog)
        {
            return ActiveDialogs.ToList().FindAll(s => s.Participants.First().Equals(dialog.Participants.First())).Count > 0;
        }
    }
}
