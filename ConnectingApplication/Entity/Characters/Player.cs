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

namespace ConnectingApplication.Characters
{
    [Serializable]
    public class Player : Character
    {
        public enum ContactType
        {
            Phone, Flype, FF, Email
        }
        public enum MessageType
        {
            Sms, Email
        }


        private Dictionary<string, Queue<DialogueNode>> textMessages;
        private Dictionary<string, Queue<DialogueNode>> emailMessages;
        private Dictionary<ContactType, List<string>> contacts;
        private List<string> files;


        public Player()
        {
            contacts = new Dictionary<ContactType, List<string>>()
            {
                { ContactType.Phone,    new List<string>()},
                { ContactType.Email,    new List<string>()},
                { ContactType.Flype,    new List<string>()},
                { ContactType.FF,       new List<string>()},
            };
            files = new List<string>();
            textMessages = new Dictionary<string, Queue<DialogueNode>>();
        }


        public Queue<DialogueNode> GetMessageHistory(string charId, MessageType messageType)
        {
            var temp = messageType == MessageType.Sms ? textMessages : emailMessages;

            if (!temp.ContainsKey(charId))
                return new Queue<DialogueNode>();
            return new Queue<DialogueNode>(temp[charId]);
        }

        public void SaveMessageHistory(BinaryWriter writer)
        {
            writer.Write(textMessages.Count);
            foreach (string name in textMessages.Keys)
            {
                writer.Write(name);
                writer.Write(textMessages[name].Count);
                foreach (DialogueNode mes in textMessages[name])
                {
                    writer.Write(mes.Id);
                }
            }
            writer.Write(emailMessages.Count);
            foreach (string name in emailMessages.Keys)
            {
                writer.Write(name);
                writer.Write(emailMessages[name].Count);
                foreach (DialogueNode mes in emailMessages[name])
                {
                    writer.Write(mes.Id);
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
                textMessages.Add(contact, new Queue<DialogueNode>());
                for (int i = 0; i < smsQueueCount; i++)
                {
                    int nodeId = reader.ReadInt32();
                    List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(Id, BlockType.body, EGetDialogueNodeType.next, nodeId);
                    textMessages[contact].Enqueue(nodes.Find(n => n.Id == nodeId));
                }
            }
            int emailCount = reader.ReadInt32();
            for (int j = 0; j < smsCount; j++)
            {
                string contact = reader.ReadString();
                int emailQueueCount = reader.ReadInt32();
                emailMessages.Add(contact, new Queue<DialogueNode>());
                for (int i = 0; i < emailQueueCount; i++)
                {
                    int nodeId = reader.ReadInt32();
                    List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(Id, BlockType.body, EGetDialogueNodeType.next, nodeId);
                    emailMessages[contact].Enqueue(nodes.Find(n => n.Id == nodeId));
                }
            }
        }

        public void AddMessage(string charId, DialogueNode dialogueNode, FormatDialogue mode)
        {
            Dictionary<string, Queue<DialogueNode>> temp = new Dictionary<string, Queue<DialogueNode>>();

            temp = mode == FormatDialogue.sms ? textMessages : emailMessages;

            if (!temp.ContainsKey(charId))
            {
                temp.Add(charId, new Queue<DialogueNode>());
                temp[charId].Enqueue(dialogueNode);
            }
            else
            {
                Queue<DialogueNode> tempQ = new Queue<DialogueNode>(temp[charId].Reverse());
                DialogueNode tempNode = tempQ.Peek();
                if (tempNode.Id != dialogueNode.Id)
                {
                    temp[charId].Enqueue(dialogueNode);
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

        public void RemoveFile(string character)
        {
            files.Remove(character);
        }

        public void RemoveContact(string character, ContactType contactType)
        {
            if (contacts[contactType].Contains(character))
                contacts[contactType].Remove(character);
        }
    }
}