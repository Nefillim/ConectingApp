using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core.Dialogues.DialogueParameters;
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
            {
                if (dialog.CharacterDialogue == NatureOfTheDialogue.discuss)
                {
                    DialogManager.Discussions.Add(dialog);
                }
                else ((NPC)Characters[character]).AddDialog(dialog);
            }
		}

		public static Dialog GetDialog(string characterId, DialogueMode mode)
		{
            return ((NPC)Characters[characterId]).GetActualDialog(mode);
		}

		public static List<string> GetCharacterInfo(string charId)
		{
			return ((NPC)Characters[charId]).CharacterInfo;
		}
	}
}
