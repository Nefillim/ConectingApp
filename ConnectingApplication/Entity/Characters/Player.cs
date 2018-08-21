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
		private Dictionary<string, Queue<DialogueNode>> textMessages;

		public Queue<DialogueNode> GetMessageHistory(string charId)
		{
			return textMessages[charId];
		}

        public void AddMessage(string charId, DialogueNode dialogueNode)
        {
            textMessages[charId].Enqueue(dialogueNode);
        }

        public IList<string> GetPhoneContacts()
        {
            return phoneContacts.AsReadOnly();
        }
	}
}
