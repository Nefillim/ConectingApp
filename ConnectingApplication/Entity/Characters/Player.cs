﻿using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
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


		public Player()
		{
			phoneContacts = new List<string>();
			textMessages = new Dictionary<string, Queue<DialogueNode>>();
		}


		public Queue<DialogueNode> GetMessageHistory(string charId)
		{
			if (!textMessages.ContainsKey(charId))
				return new Queue<DialogueNode>();
			return new Queue<DialogueNode>(textMessages[charId]);
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

		public void AddContact(string character)
		{
			phoneContacts.Add(character);
		}
	}
}