using Assets.ConectingApp.ConnectingApplication.Entity;
using Assets.ConectingApp.ConnectingApplication.Enums;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConnectingApplication.Entity.Dialog;

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

		public struct TextMes
		{
			public DialogueNode node;
			public string dialogId;
		}

		private readonly Dictionary<string, Queue<TextMes>> TextMessages;
		private readonly Queue<MessNode> emailMessages;
		private readonly Dictionary<ContactType, List<string>> contacts;
		private readonly List<string> files;
		private readonly Queue<Transaction> transactionsHistory;

		private readonly Dictionary<string, Queue<DialogueNode>> textMessages;

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
			TextMessages = new Dictionary<string, Queue<TextMes>>();
            textMessages = new Dictionary<string, Queue<DialogueNode>>();
            emailMessages = new Queue<MessNode>();
            transactionsHistory = new Queue<Transaction>();
        }


        public Queue<DialogueNode> GetMessageHistory(string charId)
        {
            if (!textMessages.ContainsKey(charId))
                return new Queue<DialogueNode>();
            return new Queue<DialogueNode>(textMessages[charId]);
        }

        public void SaveTransaction(string transactionId, float value)
        {
            transactionsHistory.Enqueue(new Transaction(transactionId, value));
        }

        public Queue<MessNode> GetEmailsHistory()
        {
            return new Queue<MessNode>(emailMessages);
        }

		public void SaveMessageHistory(BinaryWriter writer)
		{
			writer.Write(TextMessages.Count);
			foreach (string name in TextMessages.Keys)
			{
				writer.Write(name);
				writer.Write(TextMessages[name].Count);
				foreach (TextMes mes in TextMessages[name])
				{
					writer.Write(mes.node.Id);
					writer.Write(mes.dialogId);
				}
			}
			writer.Write(emailMessages.Count);
			foreach (MessNode mes in emailMessages)
			{
				writer.Write(mes.node.Id);
				writer.Write(mes.date);
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
				TextMessages.Add(contact, new Queue<TextMes>());
				for (int i = 0; i < smsQueueCount; i++)
				{
					int nodeId = reader.ReadInt32();
					string DialogId = reader.ReadString();
					List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(DialogId, BlockType.body, EGetDialogueNodeType.next, nodeId);
					DialogueNode tempNode = nodes.Find(n => n.Id == nodeId);
					TextMes tempMes = new TextMes() { node = tempNode, dialogId = DialogId};
					textMessages[contact].Enqueue(tempNode);
					TextMessages[contact].Enqueue(tempMes);
				}
			}
			int emailCount = reader.ReadInt32();
			for (int j = 0; j < smsCount; j++)
			{
				int emailQueueCount = reader.ReadInt32();
				for (int i = 0; i < emailQueueCount; i++)
				{
					int nodeId = reader.ReadInt32();
					int date = reader.ReadInt32();
					List<DialogueNode> nodes = CoreController.DialogueManager.GetNodesForDialogue(Id, BlockType.body, EGetDialogueNodeType.next, nodeId);
					MessNode mes = new MessNode()
					{
						node = nodes.Find(n => n.Id == nodeId),
						date = CoreController.TimeModule.GetDate()
					};
					emailMessages.Enqueue(mes);
				}
			}
		}

		public void AddMessage(string charId, DialogueNode dialogueNode, MessageType mode, string dialogId)
        {
            DialogueNode tempNode;
			TextMes tempSms;
            switch (mode)
            {
                case MessageType.Sms:
					TextMes tempMess = new TextMes();
					tempMess.node = dialogueNode;
					tempMess.dialogId = dialogId;

					if (!textMessages.ContainsKey(charId))
					{
						textMessages.Add(charId, new Queue<DialogueNode>());
						textMessages[charId].Enqueue(dialogueNode);
						TextMessages.Add(charId, new Queue<TextMes>());
						TextMessages[charId].Enqueue(tempMess);
					}
					else
					{
						Queue<DialogueNode> temp = new Queue<DialogueNode>(textMessages[charId].Reverse());
						Queue<TextMes> tempText = new Queue<TextMes>(TextMessages[charId].Reverse());
						if (temp.Count > 0)
						{
							tempNode = temp.Peek();
							tempSms = tempText.Peek();
							if (tempNode.Id != dialogueNode.Id)
							{
								textMessages[charId].Enqueue(dialogueNode);
								TextMessages[charId].Enqueue(tempMess);
							}
						}
						else
						{
							textMessages[charId].Enqueue(dialogueNode);
							TextMessages[charId].Enqueue(tempMess);
						}
					}
                    break;
                case MessageType.Email:
					Queue<MessNode> tempQ = new Queue<MessNode>(emailMessages.Reverse());
					if (tempQ.Count > 0)
					{
						tempNode = tempQ.Peek().node;
						if (tempNode.Id != dialogueNode.Id)
						{
							MessNode mes = new MessNode()
							{
								node = dialogueNode,
								date = CoreController.TimeModule.GetDate()
							};
							emailMessages.Enqueue(mes);
						}
					}
					else
					{
						MessNode mes = new MessNode()
						{
							node = dialogueNode,
							date = CoreController.TimeModule.GetDate()
						};
						emailMessages.Enqueue(mes);
					}
                    break;
                default:
                    break;
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

        public Queue<Transaction> GetTransactions()
        {
            return new Queue<Transaction>(transactionsHistory);
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