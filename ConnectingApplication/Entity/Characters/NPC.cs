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
        private Dictionary<DialogueMode, List<Dialog>> availableDialogs;


        public Emotion state;
        public Dictionary<string, Relationship> Relationships;

        public IList<string> CharacterInfo { get { return characterInfo.AsReadOnly(); } }


        public NPC(string id) : base(id)
        {
            availableDialogs = new Dictionary<DialogueMode, List<Dialog>>();
            characterInfo = new List<string>();
        }


        private void TryToStartDialog(Dialog d)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.TryToStartDialogue, new List<string>() { Id, d.Id, ((int)d.DialogueMode).ToString() });
        }


        public void AddDialog(Dialog d)
        {
            if (!availableDialogs.ContainsKey(d.DialogueMode))
                availableDialogs.Add(d.DialogueMode, new List<Dialog>());

			if (d.CharacterDialogue == NatureOfTheDialogue.express || d.Outgoing)
			{
				if (!availableDialogs[d.DialogueMode].Contains(d))
				{
					availableDialogs[d.DialogueMode].Insert(0, d);
					if (!d.Outgoing && ConnectingAppManager.DialogManager.ActiveDialogs.ToList().Find(s => s.Participants.Contains(Id)) != null)
					{
						TryToStartDialog(d);
					}
				}
			}
			else if (!availableDialogs[d.DialogueMode].Contains(d)) { availableDialogs[d.DialogueMode].Add(d); }

            if (d.Outgoing)
                TryToStartDialog(d);

            ActivateObject(true, d.DialogueMode);
        }

        public Dialog GetActualDialog(DialogueMode dialogueMode)
        {
            if (availableDialogs.ContainsKey(dialogueMode) && availableDialogs[dialogueMode].Count > 0)
                return availableDialogs[dialogueMode].First();
            else return null;
        }

        public void ActivateObject(bool activate, DialogueMode dialogueMode)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateCharacter,
                                                  new List<string>() { Id, activate ? "1" : "0", ((int)dialogueMode).ToString() });
        }

        public void AddFact(string factId)
        {
            characterInfo.Add(factId);
        }

        public IList<Dialog> GetAvailableDialogs(DialogueMode dialogueMode)
        {
            if (availableDialogs.ContainsKey(dialogueMode))
                return availableDialogs[dialogueMode].AsReadOnly();
            else return new List<Dialog>();
        }

        public bool RemoveDialog(DialogueMode dialogueMode, Dialog dialog)
        {
            if (availableDialogs.ContainsKey(dialogueMode))
                return availableDialogs[dialogueMode].Remove(dialog);
            else return false;
        }
    }
}