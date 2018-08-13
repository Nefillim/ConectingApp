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
        public static Dictionary<string, Character> Characters = new Dictionary<string, Character>();


		public static void AddDialog(Dialog dialog, string character)
		{
            if (Characters.ContainsKey(character) && Characters[character] is NPC)
                ((NPC)Characters[character]).AddDialog(dialog);
		}

		public static List<Dialog> GetDialogs(string character)
		{
			if (Characters.ContainsKey(character) && Characters[character] is NPC)
				return ((NPC)Characters[character]).GetDialogs();
			else
				return null;
		}

		public static Dialog GetDialog(string characterId, string dialogId)
		{
			return GetDialogs(characterId).Find(d => d.id == dialogId);
		}

		public static List<string> GetCharacterInfo(string charId)
		{
			return ((NPC)Characters[charId]).CharacterInfo;
		}
	}
}
