using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public class CharacterManager
    {
        private Dictionary<string, Character> Characters;


        [Obsolete("Don't use outside the ConnectingApp.")]
        public CharacterManager()
        {
            Characters = new Dictionary<string, Character>()
            {
                {ConnectingAppManager.PLAYER_ID, new Player() }
            };
        }

        public void AddDialog(Dialog dialog, string character)
        {
            if (!Characters.ContainsKey(character))
            {
                Characters.Add(character, new NPC(character));
                GetPlayer().AddContact(character);
            }
            if (Characters[character] is NPC)
            {
                if (dialog.CharacterDialogue == NatureOfTheDialogue.discuss)
                {
                    ConnectingAppManager.DialogManager.AddDiscussion(dialog);
                }
                else ((NPC)Characters[character]).AddDialog(dialog);
            }
            else Debug.LogError("Trying to add dialog to player");
        }

        public Dialog GetDialog(string characterId, DialogueMode mode)
        {
            return ((NPC)Characters[characterId]).GetActualDialog(mode);
        }

        public List<string> GetCharacterInfo(string charId)
        {
            return ((NPC)Characters[charId]).CharacterInfo;
        }

        public Player GetPlayer()
        {
            return Characters[ConnectingAppManager.PLAYER_ID] as Player;
        }

        public NPC GetNPC(string characterId)
        {
            return Characters[characterId] as NPC;
        }
    }
}
