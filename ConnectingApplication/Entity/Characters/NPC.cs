using Assets.ConectingApp.ConnectingApplication;
using Assets.ConectingApp.ConnectingApplication.Enums;
using Assets.Scripts;
using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using ConnectingApplication.Managers;
using Core.Dialogues.DialogueParameters;
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
        private List<string> characterInfo;
        private Dictionary<FormatDialogue, List<Dialog>> availableDialogs;


        public Emotion state;
        public Dictionary<string, Relationship> Relationships;

        public IList<string> CharacterInfo { get { return characterInfo.AsReadOnly(); } }


        public NPC(string id) : base(id)
        {
            availableDialogs = new Dictionary<FormatDialogue, List<Dialog>>();
            characterInfo = new List<string>();
        }


        private void TryToStartDialog(Dialog d)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.TryToStartDialogue, new List<string>() { Id, d.Id, ((int)d.Format).ToString() });
        }


        public void AddDialog(Dialog d)
        {
            if (!availableDialogs.ContainsKey(d.Format))
                availableDialogs.Add(d.Format, new List<Dialog>());

			if (d.CharacterOfDialogue == CharacterOfDialogue.express || d.Outgoing)
			{
				if (!availableDialogs[d.Format].Contains(d))
				{
					availableDialogs[d.Format].Insert(0, d);
					if (!d.Outgoing && ConnectingAppManager.DialogManager.ActiveDialogs.ToList().Find(s => s.Participants.Contains(Id)) != null)
					{
						TryToStartDialog(d);
					}
				}
			}
			else if (!availableDialogs[d.Format].Contains(d)) { availableDialogs[d.Format].Add(d); }

            if (d.Outgoing)
                TryToStartDialog(d);

            ActivateObject(true, d.Format);
        }

        public void RemoveDialog(Dialog d)
        {
            availableDialogs[d.Format].Remove(d);
        }

        public Dialog GetActualDialog(FormatDialogue dialogueMode)
        {
            if (availableDialogs.ContainsKey(dialogueMode) && availableDialogs[dialogueMode].Count > 0)
                return availableDialogs[dialogueMode].First();
            else return null;
        }

        public void ActivateObject(bool activate, FormatDialogue dialogueMode)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateCharacter,
                                                  new List<string>() { Id, activate ? "1" : "0", ((int)dialogueMode).ToString() });
        }

        public void AddFact(string factId)
        {
            characterInfo.Add(factId);
        }

        public IList<Dialog> GetAvailableDialogs(FormatDialogue dialogueMode)
        {
            if (availableDialogs.ContainsKey(dialogueMode))
                return availableDialogs[dialogueMode].AsReadOnly();
            else return new List<Dialog>();
        }

        public bool RemoveDialog(FormatDialogue dialogueMode, Dialog dialog)
        {
            if (availableDialogs.ContainsKey(dialogueMode))
                return availableDialogs[dialogueMode].Remove(dialog);
            else return false;
        }
    }
}