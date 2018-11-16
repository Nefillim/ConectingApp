using Assets.ConectingApp.ConnectingApplication.Enums;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConnectingApplication.Entity.Dialog;

namespace ConnectingApplication.Characters
{
    public class Player : Character
    {
        public enum ContactType
        {
            Phone, Flype, FF
        }


        private Dictionary<string, Queue<MessNode>> textMessages;
        private Dictionary<string, Queue<MessNode>> emailMessages;
        private Dictionary<ContactType, List<string>> contacts;
        private List<string> files;


        public Player()
        {
            contacts = new Dictionary<ContactType, List<string>>()
            {
                { ContactType.Phone,    new List<string>()},
                { ContactType.Flype,    new List<string>()},
                { ContactType.FF,       new List<string>()},
            };
            files = new List<string>();
            textMessages = new Dictionary<string, Queue<MessNode>>();
        }


        public Queue<MessNode> GetMessageHistory(string charId)
        {
            if (!textMessages.ContainsKey(charId))
                return new Queue<MessNode>();
            return new Queue<MessNode>(textMessages[charId]);
        }

        public void SaveMessageHistory(BinaryWriter writer)
        {
            writer.Write(textMessages.Count);
            foreach (string name in textMessages.Keys)
            {
                writer.Write(name);
                writer.Write(textMessages[name].Count);
                foreach (MessNode mes in textMessages[name])
                {
                    writer.Write(mes.node.Id);
					writer.Write(mes.date);
                }
            }
            writer.Write(emailMessages.Count);
            foreach (string name in emailMessages.Keys)
            {
                writer.Write(name);
                writer.Write(emailMessages[name].Count);
                foreach (MessNode mes in emailMessages[name])
                {
					writer.Write(mes.node.Id);
					writer.Write(mes.date);
				}
            }
        }

        public void DownloadMessageHistory(BinaryReader reader)
        {
            int smsCount = reader.ReadInt32();
            for (int j = 0; j < smsCount; j++)
            {
                string contact = reader.ReadString();
                int smsQueueCount = reader.ReadInt32();
                textMessages.Add(contact, new Queue<MessNode>());
                for (int i = 0; i < smsQueueCount; i++)
                {
                    int nodeId = reader.ReadInt32();
					int _date = reader.ReadInt32();
					List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(Id, BlockType.body, EGetDialogueNodeType.next, nodeId);
					MessNode mes = new MessNode() {
						node = nodes.Find(n => n.Id == nodeId),
						date = _date
					};
					textMessages[contact].Enqueue(mes);
                }
            }
            int emailCount = reader.ReadInt32();
            for (int j = 0; j < smsCount; j++)
            {
                string contact = reader.ReadString();
                int emailQueueCount = reader.ReadInt32();
                emailMessages.Add(contact, new Queue<MessNode>());
                for (int i = 0; i < emailQueueCount; i++)
                {
                    int nodeId = reader.ReadInt32();
					int _date = reader.ReadInt32();
                    List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(Id, BlockType.body, EGetDialogueNodeType.next, nodeId);
					MessNode mes = new MessNode()
					{
						node = nodes.Find(n => n.Id == nodeId),
						date = _date
					};
					textMessages[contact].Enqueue(mes);
				}
            }
        }

        public void AddMessage(string charId, DialogueNode dialogueNode, FormatDialogue mode)
        {
            Dictionary<string, Queue<MessNode>> temp = new Dictionary<string, Queue<MessNode>>();

            temp = mode == FormatDialogue.sms ? textMessages : emailMessages;

            if (!temp.ContainsKey(charId))
            {
				MessNode mes = new MessNode() {
					node = dialogueNode,
					date = CoreController.TimeModule.GetDate()
				};
                temp.Add(charId, new Queue<MessNode>());
                temp[charId].Enqueue(mes);
            }
            else
            {
                Queue<MessNode> tempQ = new Queue<MessNode>(temp[charId].Reverse());
                MessNode tempNode = tempQ.Peek();
                if (tempNode.node.Id != dialogueNode.Id)
                {
					MessNode mes = new MessNode()
					{
						node = dialogueNode,
						date = CoreController.TimeModule.GetDate()
					};
					temp[charId].Enqueue(mes);
                }
            }
        }

        public IList<string> GetContacts(ContactType contactType)
        {
            return contacts[contactType].AsReadOnly();
        }

        public IList<string> GetFiles()
        {
            return files.AsReadOnly();
        }

        public void AddContact(string character, ContactType contactType)
        {
            if (!contacts[contactType].Contains(character))
                contacts[contactType].Add(character);
        }

        public void AddFile(string character)
        {
            files.Add(character);
        }

        public void RemoveContact(string character, ContactType contactType)
        {
            if (contacts[contactType].Contains(character))
                contacts[contactType].Remove(character);
        }
    }
}