using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
	public static class CharacterManager
	{
		public static List<Character> Characters = new List<Character>();
		public static void AddDialog(Dialog dialog, NPC character)
		{
			character.AddDialog(dialog);
		}
		public static List<Dialog> GetDialogs(string character)
		{
			if (Characters.Find(ch => ch.id == character) is NPC)
				return ((NPC)Characters.Find(ch => ch.id == character)).GetDialogs();
			else
				return null;
		}

		public static Dialog GetDialog(string characterId, string dialogId)
		{
			return GetDialogs(characterId).Find(d => d.id == dialogId);
		}

	}
}
