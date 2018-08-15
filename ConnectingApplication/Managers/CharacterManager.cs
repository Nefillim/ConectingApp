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
	public class CharacterManager 
    {
        public Dictionary<string, Character> Characters;


        public CharacterManager()
        {
            Characters = new Dictionary<string, Character>();
        }

		public void AddDialog(Dialog dialog, string character)
		{
            if (Characters.ContainsKey(character) && Characters[character] is NPC)
            {
                if (dialog.CharacterDialogue == NatureOfTheDialogue.discuss)
                {
                    ConnectingAppManager.DialogManager.Discussions.Add(dialog);
                }
                else ((NPC)Characters[character]).AddDialog(dialog);
            }
		}

		public Dialog GetDialog(string characterId, DialogueMode mode)
		{
            return ((NPC)Characters[characterId]).GetActualDialog(mode);
		}

		public List<string> GetCharacterInfo(string charId)
		{
			return ((NPC)Characters[charId]).CharacterInfo;
		}
	}
}
