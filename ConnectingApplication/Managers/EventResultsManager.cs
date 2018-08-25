﻿using Assets.ConectingApp.ConnectingApplication.Enums;
using Assets.Scripts;
using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public class EventResultsManager
    {
        private static readonly Dictionary<string, ResultFuncsEnum> funcs = new Dictionary<string, ResultFuncsEnum>()
        {
            {"NextSlot",            ResultFuncsEnum.NextSlot },
            {"ActivateBusiness",    ResultFuncsEnum.ActivateBusiness },
            {"ActivateDialogue",    ResultFuncsEnum.ActivateDialogue },
            {"StartMiniGame",       ResultFuncsEnum.StartMiniGame },
            {"PlayMusic",           ResultFuncsEnum.PlayMusic },
            {"OpenFile",            ResultFuncsEnum.OpenFile},
            {"OpenFact",            ResultFuncsEnum.OpenFact},
            {"ChangeInitiative",    ResultFuncsEnum.ChangeInitiative},
            {"ActivateObject",      ResultFuncsEnum.ActivateObject},
            {"StartBusiness",       ResultFuncsEnum.StartBusiness},
            {"GetChoose",           ResultFuncsEnum.GetChoose},

        };
        private static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs =
            new Dictionary<ResultFuncsEnum, Action<List<string>>>()
            {
                { ResultFuncsEnum.ActivateDialogue,     OnNewAvailableDialog },
                { ResultFuncsEnum.ActivateBusiness,     ActivateBusiness },
                { ResultFuncsEnum.PlayMusic,            PlayMusic },
                { ResultFuncsEnum.NextSlot,             NextSlot },
                { ResultFuncsEnum.StartMiniGame,        StartMiniGame },
                { ResultFuncsEnum.ChangeInitiative,     ChangeInitiative},
                { ResultFuncsEnum.OpenFact,             OpenFact},
                { ResultFuncsEnum.OpenFile,             OpenFile},
                { ResultFuncsEnum.Error,                Error},
                { ResultFuncsEnum.ActivateObject,       ActivateObject},
                { ResultFuncsEnum.StartBusiness,        StartBusiness},
                { ResultFuncsEnum.GetChoose,            GetChoose},
            };


        [Obsolete("Don't use outside the ConnectingApp.")]
        public EventResultsManager()
        {

        }


        private static ResultFuncsEnum Build(string func)
        {
            if (!funcs.ContainsKey(func))
            {
                Debug.LogError($"Среди предписанных методов не найден метод: {func}.");
                return ResultFuncsEnum.Error;
            }

            ResultFuncsEnum enumerator = funcs[func];
            return enumerator;
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

        private static void GetChoose(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.GetChoose, input);
        }

        private static void StartBusiness(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.StartBusiness, input);

        }

        private static void ActivateObject(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateObject, input);
        }

        private static void ActivateBusiness(List<string> businesses)
        {
            foreach (var b in businesses)
                ConnectingAppManager.BusinessManager.AddAvailableBusiness(b);
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
            var character = ConnectingAppManager.CharacterManager.GetNPC(facts.First());
            character.CharacterInfo.Add(facts.Last());
        }

        private static void PlayMusic(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.PlayMusic, input);
        }

        private static void NextSlot(List<string> input)
        {
            Debug.LogError("Метод NextSlot не реализован.");
        }

        private static void StartMiniGame(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.StartMiniGame, input);
        }

        private static void ChangeInitiative(List<string> input)
        {
            Debug.LogError("Метод ChangeInitiative не реализован.");
        }

        private static void OpenFact(List<string> input)
        {
            ConnectingAppManager.CharacterManager.GetNPC(input[0]).AddFact(input[1]);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.OpenFact, input);
        }

        private static void OpenFile(List<string> input)
        {
            ConnectingAppManager.CharacterManager.AddContact(input[0]);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.OpenFile, input);
        }

        private static void Error(List<string> input)
        {
            Debug.LogError("Необходимо реализовать новый метод для ивентов.");
        }


        public void CoreEventsResult(string enumerator, List<string> fields)
        {
            string inputFields = string.Empty;
            foreach (var i in fields)
            {
                inputFields = string.Concat(inputFields, i, ", ");
            }
            inputFields = inputFields.Remove(inputFields.Length - 2);
            Debug.Log($"ResultType: {enumerator.ToString()}({inputFields})");

            var func = Build(enumerator);
            if (!ResultFuncs.ContainsKey(func))
            {
                Debug.LogError($"Среди предписанных методов не найден метод: {func}.");
                return;
            }
            ResultFuncs[func].Invoke(fields);
        }
    }
}