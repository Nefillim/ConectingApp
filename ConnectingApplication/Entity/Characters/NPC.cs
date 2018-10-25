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
        private Dictionary<FormatDialogue, List<Dialog>> deactivatedDialogs;
        private string idModificator;


        public Emotion state;
        public Dictionary<string, Relationship> Relationships;
        public readonly Dictionary<FormatDialogue, bool> Availability = new Dictionary<FormatDialogue, bool>()
        {
            { FormatDialogue.call,              false },
            { FormatDialogue.dialogueInterface, false },
            { FormatDialogue.sms,               false },
            { FormatDialogue.email,             false },
            { FormatDialogue.meet,              false },
            { FormatDialogue.videocall,         false },
        };

        public IList<string> CharacterInfo { get { return characterInfo.AsReadOnly(); } }


        public NPC(string id) : base(id)
        {
            availableDialogs = new Dictionary<FormatDialogue, List<Dialog>>();
            deactivatedDialogs = new Dictionary<FormatDialogue, List<Dialog>>();
            characterInfo = new List<string>();
            ConnectingAppManager.BusinessManager.ChangedBusinesses += RemoveDialogs;
            ConnectingAppManager.BusinessManager.NewBusiness += ChangeDialogsAvailability;
        }


        private void TryToStartDialog(Dialog d)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.TryToStartDialogue, new List<string>() { Id, d.Id, ((int)d.Format).ToString() });
        }

        private void ActivateObject(bool activate, FormatDialogue format)
        {
            //TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateCharacter, new List<string>() { Id, activate ? "1" : "0", ((int)format).ToString() });
            Availability[format] = activate;
        }

        /// <summary>
        /// Удаляет все диалоги которые должны происходить в указанных занятиях.
        /// </summary>
        /// <param name="businessId"></param>
        private void RemoveDialogs(List<string> businessesId)
        {
            foreach (var i in availableDialogs)
            {
                foreach (var j in businessesId)
                {
                    i.Value.RemoveAll(s => s.BusinessId.Equals(j));
                }
                if (GetAvailableDialogs(i.Key).Count == 0)
                    ActivateObject(false, i.Key);
            }
        }

        private void ChangeDialogsAvailability(string oldBusinessId, string newBusinessId)
        {
            ChangeDictionaryForDialog(ref availableDialogs, ref deactivatedDialogs, oldBusinessId);
            ChangeDictionaryForDialog(ref deactivatedDialogs, ref availableDialogs, newBusinessId);
        }

        private void ChangeDictionaryForDialog(ref Dictionary<FormatDialogue, List<Dialog>> one, ref Dictionary<FormatDialogue, List<Dialog>> two, string formatter)
        {
            foreach (var formatListPair in one)
            {
                if (!two.ContainsKey(formatListPair.Key))
                    two.Add(formatListPair.Key, new List<Dialog>());

                for (int j = 0; j < formatListPair.Value.Count; ++j)
                {
                    if (formatListPair.Value[j].BusinessId.Equals(formatter))
                    {
                        two[formatListPair.Key].Add(formatListPair.Value[j]);
                        one[formatListPair.Key].Remove(formatListPair.Value[j]);
                    }
                }
                if (GetAvailableDialogs(formatListPair.Key).Count == 0)
                    ActivateObject(false, formatListPair.Key);
            }
        }


        public void AddDialog(Dialog d)
        {
            if (!availableDialogs.ContainsKey(d.Format))
                availableDialogs.Add(d.Format, new List<Dialog>());

            // Удаление такого же диалога, и запись нового(тип начать с начала).
            //if (availableDialogs[d.Format].Exists(s => s.Id.Equals(d.Id)))
            //availableDialogs[d.Format].RemoveAll(s => s.Id.Equals(d.Id));

            if (d.CharacterOfDialogue == CharacterOfDialogue.express || d.Outgoing)
            {
                if (!availableDialogs[d.Format].Contains(d))
                {
                    availableDialogs[d.Format].Insert(0, d);
                }
                if (!d.Outgoing && ConnectingAppManager.DialogManager.ActiveDialogs.ToList().
                        Find(s => s.Participants.Contains(Id)) != null)
                {
                    TryToStartDialog(d);
                }
            }
            else if (!availableDialogs[d.Format].Contains(d)) { availableDialogs[d.Format].Add(d); }

            if (d.Outgoing)
                TryToStartDialog(d);

            ActivateObject(true, d.Format);
        }

        public void RemoveDialog(Dialog d)
        {
            if (availableDialogs.ContainsKey(d.Format) && availableDialogs[d.Format].Contains(d))
                availableDialogs[d.Format].Remove(d);
            if (GetAvailableDialogs(d.Format).Count == 0)
                ActivateObject(false, d.Format);
        }

        public Dialog GetDialog(FormatDialogue dialogueMode, string dialogId)
        {
            if (availableDialogs.ContainsKey(dialogueMode) && availableDialogs[dialogueMode].Count > 0)
            {
                if (dialogId.Equals(""))
                    return availableDialogs[dialogueMode].First();
                else return availableDialogs[dialogueMode].Find(s => s.Id.Equals(dialogId));
            }
            else return null;
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

        public void SetIdModificator(string newModificator)
        {
            idModificator = newModificator;
        }

        public string GetCurrentId()
        {
            return string.Concat(Id, idModificator);
        }
    }
}