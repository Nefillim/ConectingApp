﻿using ConnectingApplication.Characters;
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


        private void CreateCharacter(string charId)
        {
            if (!Characters.ContainsKey(charId))
            {
                Characters.Add(charId, new NPC(charId));
            }
        }


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
            CreateCharacter(character);
            if (Characters[character] is NPC)
            {
                if (dialog.CharacterOfDialogue == CharacterOfDialogue.discuss)
                {
                    ConnectingAppManager.DialogManager.AddDiscussion(dialog);
                }
                else ((NPC)Characters[character]).AddDialog(dialog);
            }
            else Debug.LogError("Trying to add dialog to player");
        }

        public void RemoveDialog(Dialog dialog, string character)
        {
            ((NPC)Characters[character]).RemoveDialog(dialog);
        }

        public Dialog GetDialog(string characterId, FormatDialogue mode, string dialogId = "")
        {
            CreateCharacter(characterId);
            return ((NPC)Characters[characterId]).GetDialog(mode, dialogId);
        }

        public IList<string> GetCharacterInfo(string charId)
        {
            CreateCharacter(charId);
            return ((NPC)Characters[charId]).CharacterInfo;
        }

        public Player GetPlayer()
        {
            return Characters[ConnectingAppManager.PLAYER_ID] as Player;
        }

        public NPC GetNPC(string characterId)
        {
            CreateCharacter(characterId);
            return Characters[characterId] as NPC;
        }

        public void AddContact(string charId, Player.ContactType contactType)
        {
            CreateCharacter(charId);
            GetPlayer().AddContact(charId, contactType);
        }

        public void RemoveContact(string charId, Player.ContactType contactType)
        {
            GetPlayer().RemoveContact(charId, contactType);
        }

        public void CreateFile(string character)
        {
            CreateCharacter(character);
            GetPlayer().AddFile(character);
        }
    }
}
