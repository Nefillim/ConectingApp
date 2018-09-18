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
	public class Player : Character
	{
		private Dictionary<string, Queue<DialogueNode>> textMessages;
		private Dictionary<string, Queue<DialogueNode>> emailMessages;
		private List<string> phoneContacts;
		private List<string> files;
        private EBlockState actualBlockState;
        private EBlockState previusBlockState;


		public Player()
		{
			phoneContacts = new List<string>();
            files = new List<string>();
			textMessages = new Dictionary<string, Queue<DialogueNode>>();
		}


        public void SetBlockState(EBlockState blockState)
        {
            //TODO: realize;
        }

        public EBlockState GetBlockState()
        {
            return actualBlockState;
        }

		public Queue<DialogueNode> GetMessageHistory(string charId)
		{
			if (!textMessages.ContainsKey(charId))
				return new Queue<DialogueNode>();
			return new Queue<DialogueNode>(textMessages[charId]);
		}

		public void SaveMessageHistory(BinaryWriter writer)
		{
			writer.Write(textMessages.Count);
			foreach (string name in textMessages.Keys)
			{
				writer.Write(name);
				writer.Write(textMessages[name].Count);
				foreach(DialogueNode mes in textMessages[name])
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

		public void AddMessage(string charId, DialogueNode dialogueNode, DialogueMode mode)
		{
			Dictionary<string, Queue<DialogueNode>> temp = new Dictionary<string, Queue<DialogueNode>>();

			temp = mode == DialogueMode.sms ? textMessages : emailMessages;

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

		public IList<string> GetPhoneContacts()
		{
			return phoneContacts.AsReadOnly();
		}

        public IList<string> GetFiles()
        {
            return files.AsReadOnly();
        }

        public void AddContact(string character)
		{
			phoneContacts.Add(character);
		}

        public void AddFile(string character)
        {
            files.Add(character);
        }
    }
}