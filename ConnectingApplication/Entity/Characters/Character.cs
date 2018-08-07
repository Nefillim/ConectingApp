using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Entity.Characters
{
	public class Character
	{
		public string id;
		public virtual void Say(DialogueNode phrase) { }
	}
}
