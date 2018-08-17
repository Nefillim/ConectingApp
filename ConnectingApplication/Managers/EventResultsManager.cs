using Assets.Scripts;
using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
using Core.Parser.Results.ResultFunc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public class EventResultsManager 
    {
        private static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs =
            new Dictionary<ResultFuncsEnum, Action<List<string>>>()
            {
                { ResultFuncsEnum.ActivateDialogue, OnNewAvailableDialog },
                { ResultFuncsEnum.ActivateBusiness, OnChangeBusiness },
                { ResultFuncsEnum.playMusic, PlayMusic },
                { ResultFuncsEnum.nextSlot, NextSlot },
                { ResultFuncsEnum.StartMiniGame, StartMiniGame },
                { ResultFuncsEnum.error, Error },
            };


        [Obsolete("Don't use outside the ConnectingApp.")]
        public EventResultsManager()
        {

        }


        private static void OnNewAvailableDialog(List<string> dialogues)
        {
            foreach (var d in dialogues)
            {
                var dialogue = CoreController.DialogueManager.GetDialogue(d);
                Dialog dialog = new Dialog(dialogue);
                ConnectingAppManager.CharacterManager.AddDialog(dialog, dialog.Participants.First());
            }
        }

        private static void OnChangeBusiness(List<string> businesses)
        {
            if (businesses.Count > 1)
                throw new FormatException("Игрок не может выбрать больше одного занятия!");
            var business = ConnectingAppManager.BusinessManager.GetBusinessInfo(businesses.First());
            ConnectingAppManager.DownloadManager.SetNewIteratorPosition();
        }

        private static void OnNewCharacterFact(List<string> facts)
        {
            NPC character = (NPC)ConnectingAppManager.CharacterManager.Characters[facts.First()];
            character.CharacterInfo.Add(facts.Last());
        }

        private static void PlayMusic(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.playMusic, input);
        }

        private static void NextSlot(List<string> input)
        {
            throw new NotImplementedException();
        }

        private static void Error(List<string> input)
        {
            Debug.LogError("Необходимо модифицировать методы в core.");
        }

        private static void StartMiniGame(List<string> input)
        {
            throw new NotImplementedException();
        }


        public void CoreEventsResult(ResultFuncsEnum enumerator, List<string> fields)
        {
            Debug.Log($"ResultType: {enumerator.ToString()}, inputFields: {string.Concat(fields)}");
            ResultFuncs[enumerator].Invoke(fields);
        }
    }
}