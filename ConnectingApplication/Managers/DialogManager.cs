﻿using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class DialogManager
    {
        public Stack<Dialog> ActiveDialogs;
        public Dictionary<string, Dialog> ActiveMessageDialogs;
        public List<Dialog> Discussions;
        public List<Dialog> TextMessages;

        public Player player;


        [Obsolete("Don't use outside the ConnectingApp.")]
        public DialogManager()
        {
            ActiveDialogs = new Stack<Dialog>();
            ActiveMessageDialogs = new Dictionary<string, Dialog>();
        }


        public void SetResultsForNode(DialogueNode dialogueNode)
        {
            dialogueNode.InvokeResult();
            // TODO: сохранить 
        }

        /// <summary>
        /// Чтобы запросить новые ноды 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<DialogueNode> ContinueDialog(EGetDialogueNodeType nodeType, DialogueNode dialogueNode = null)
        {
            Dialog curDialog = ActiveDialogs.Peek();
            if (nodeType == EGetDialogueNodeType.next)
                SetResultsForNode(dialogueNode);
            var nodeId = nodeType == EGetDialogueNodeType.firstNodes ? -1 : dialogueNode.Id;
            var nodes = curDialog.TakeNextNodes(nodeId);
            if (nodes.Count != 0)
            {
                return nodes;
            }
            else
            {
                ActiveDialogs.Pop();
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
                else return ContinueDialog(EGetDialogueNodeType.firstNodes);
            }
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

        public void StartDialog(string charId, DialogueMode dialogueMode)
        {
            Dialog newOne = ConnectingAppManager.CharacterManager.GetDialog(charId, dialogueMode);
            newOne.currentBlock = ActiveDialogs.Count() > 0 ? Core.Dialogues.DialogueBlock.BlockType.body : Core.Dialogues.DialogueBlock.BlockType.hi;
            ActiveDialogs.Push(newOne);
        }
    }
}
