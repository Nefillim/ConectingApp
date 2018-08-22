using Assets.ConectingApp.ConnectingApplication;
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
        public Dictionary<DialogueMode, List<Dialog>> AvailableDialogs;
        public List<string> CharacterInfo;
        public Emotion state;
        public Dictionary<string, Relationship> Relationships;

        public NPC(string id) : base(id)
        {
            AvailableDialogs = new Dictionary<DialogueMode, List<Dialog>>();
        }


        private void TryToStartDialog(Dialog d)
        {
            TriangleManager.InvokeConnectionFuncs(ConnectingFuncsEnum.TryToStartDialogue, new List<string>() { Id, d.Id, ((int)d.DialogueMode).ToString()});
        }


        public void AddDialog(Dialog d)
        {
            if (!AvailableDialogs.ContainsKey(d.DialogueMode))
                AvailableDialogs.Add(d.DialogueMode, new List<Dialog>());

            if (d.CharacterDialogue == NatureOfTheDialogue.express || d.Outgoing)
            {
                AvailableDialogs[d.DialogueMode].Insert(0, d);
                if (!d.Outgoing && ConnectingAppManager.DialogManager.ActiveDialogs.ToList().Find(s => s.Participants.Contains(Id)) != null)
                {
                    TryToStartDialog(d);
                }
            }
            else AvailableDialogs[d.DialogueMode].Add(d);

            if (d.Outgoing)
                TryToStartDialog(d);

            ActivateObject(true, d.DialogueMode);
        }

        public Dialog GetActualDialog(DialogueMode dialogueMode)
        {
            if (AvailableDialogs.ContainsKey(dialogueMode) && AvailableDialogs[dialogueMode].Count > 0)
                return AvailableDialogs[dialogueMode].First();
            else return null;
        }

        public void ActivateObject(bool activate, DialogueMode dialogueMode)
        {
            TriangleManager.InvokeConnectionFuncs(ConnectingFuncsEnum.ActivateObject, 
                                                  new List<string>() { Id, activate ? "1" : "0", ((int)dialogueMode).ToString() });
        }
    }
}