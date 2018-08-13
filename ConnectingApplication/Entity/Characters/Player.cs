using ConnectingApplication.Entity;
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
		public Dictionary<string, Queue<DialogueNode>> textMessages;
		public List<string> PhoneContacts;

		public Queue<DialogueNode> GetMessageHistory(string charId)
		{
			return textMessages[charId];
		}
	}
}
