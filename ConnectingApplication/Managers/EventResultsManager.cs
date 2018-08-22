using Assets.ConectingApp.ConnectingApplication.Enums;
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
            {"ChangeIIitiative",    ResultFuncsEnum.ChangeInitiative},
        };
        private static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs =
            new Dictionary<ResultFuncsEnum, Action<List<string>>>()
            {
                { ResultFuncsEnum.ActivateDialogue,     OnNewAvailableDialog },
                { ResultFuncsEnum.ActivateBusiness,     OnChangeBusiness },
                { ResultFuncsEnum.PlayMusic,            PlayMusic },
                { ResultFuncsEnum.NextSlot,             NextSlot },
                { ResultFuncsEnum.StartMiniGame,        StartMiniGame },
                { ResultFuncsEnum.ChangeInitiative,     ChangeInitiative},
                { ResultFuncsEnum.OpenFact,             OpenFact},
                { ResultFuncsEnum.OpenFile,             OpenFile},
                { ResultFuncsEnum.Error,                Error},
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
            Debug.LogError("Метод не реализован.");
        }

        private static void StartMiniGame(List<string> input)
        {
            Debug.LogError("Метод не реализован.");
        }

        private static void ChangeInitiative(List<string> input)
        {
            Debug.LogError("Метод не реализован.");
        }

        private static void OpenFact(List<string> input)
        {
            Debug.LogError("Метод не реализован.");
        }

        private static void OpenFile(List<string> input)
        {
            Debug.LogError("Метод не реализован.");
        }

        private static void Error(List<string> input)
        {
            Debug.LogError("Необходимо реализовать новый метод для ивентов.");
        }


        public void CoreEventsResult(string enumerator, List<string> fields)
        {
            Debug.Log($"ResultType: {enumerator.ToString()}, inputFields: {string.Concat(fields)}");
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