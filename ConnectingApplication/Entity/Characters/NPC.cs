using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Characters
{
	public enum Emotion
	{
		kind,
		angry,
		bored,
		interested,
		scared
	}

	public enum Relationship
	{
		friend,
		normal,
		opposition,
		enemy,
	}

	public class NPC : Character
	{
        public List<Dialog> AvailableDialogs = new List<Dialog>();

        public void AddDialog(Dialog d)
		{
			AvailableDialogs.Add(d);
		}
		public List<Dialog> GetDialogs()
		{
			return AvailableDialogs;
		}

		public List<string> CharacterInfo;

		public Emotion state;

		public Dictionary<string, Relationship> Relationships;
			
	}
}
