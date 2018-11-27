using Assets.ConectingApp.ConnectingApplication.Enums;
using Assets.Scripts;
using Assets.Scripts.Helpers;
using ConnectingApplication.Characters;
using ConnectingApplication.Entity;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueBlock;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public class EventResultsManager //TODO: for all methods using view methods make if(!saveMode) condition
    {
        private static readonly Dictionary<string, ResultFuncsEnum> funcs = new Dictionary<string, ResultFuncsEnum>()
        {
            { "NextSlot",            ResultFuncsEnum.NextSlot},
            { "ActivateBusiness",    ResultFuncsEnum.ActivateBusiness},
            { "ActivateDialogue",    ResultFuncsEnum.ActivateDialogue},
            { "StartMiniGame",       ResultFuncsEnum.StartMiniGame},
            { "PlayMusic",           ResultFuncsEnum.PlayMusic},
            { "OpenFile",            ResultFuncsEnum.OpenFile},
            { "OpenFact",            ResultFuncsEnum.OpenFact},
            { "ChangeParameter",     ResultFuncsEnum.ChangeParameter},

            { "TryToStartDialogue",  ResultFuncsEnum.TryToStartDialogue},
            { "ActivateObject",      ResultFuncsEnum.ActivateObject},

            { "StartBusiness",       ResultFuncsEnum.StartBusiness},
            { "GetChoose",           ResultFuncsEnum.GetChoose},

            { "AddToContactList",    ResultFuncsEnum.AddToContactList},
            { "AddToFlype",          ResultFuncsEnum.AddToFlype},
            { "AddToFF",             ResultFuncsEnum.AddToFF},
            { "DeleteContact",       ResultFuncsEnum.DeleteContact},
            { "DeleteContactFF",     ResultFuncsEnum.DeleteContactFF},
            { "DeactivateBusiness",  ResultFuncsEnum.DeactivateBusiness},
            { "ActivateMiniGame",    ResultFuncsEnum.ActivateMiniGame},
            { "DeactivateMiniGame",  ResultFuncsEnum.DeactivateMiniGame},
            { "DeactivateDialogue",  ResultFuncsEnum.DeactivateDialogue},
            { "StartDialogue",       ResultFuncsEnum.StartDialogue},
            { "AddChar",             ResultFuncsEnum.AddChar},
            { "GoTo",                ResultFuncsEnum.GoTo},
            { "AddTask",             ResultFuncsEnum.AddTask},
            { "SayReplic",           ResultFuncsEnum.SayReplic},
            { "Fade",                ResultFuncsEnum.Fade},
            { "ChangeLocState",      ResultFuncsEnum.ChangeLocState},
            { "Change",              ResultFuncsEnum.Change},
            { "Teleport",            ResultFuncsEnum.Teleport},
            { "ShowTask",            ResultFuncsEnum.ShowTask},
            { "CloseTask",           ResultFuncsEnum.CloseTask},
            { "ShowSubtitles",       ResultFuncsEnum.ShowSubtitles},
            { "ShowNotification",    ResultFuncsEnum.ShowNotification},
            { "ChangeState",         ResultFuncsEnum.ChangeState},
            { "DeleteProfile",       ResultFuncsEnum.DeleteProfile},
            { "ActivateDevice",      ResultFuncsEnum.ActivateDevice},
        };
        private static readonly Dictionary<string, EChangingParameter> parameters = new Dictionary<string, EChangingParameter>()
        {
            {"Initiative",      EChangingParameter.Initiative },
            {"WaitingSeconds",  EChangingParameter.WaitingSeconds }
        };
        private static readonly Dictionary<ResultFuncsEnum, Action<List<string>>> ResultFuncs = new Dictionary<ResultFuncsEnum, Action<List<string>>>()
        {
            { ResultFuncsEnum.Error,                Error},
            { ResultFuncsEnum.NextSlot,             NextSlot },
            { ResultFuncsEnum.ActivateBusiness,     ActivateBusiness },
            { ResultFuncsEnum.ActivateDialogue,     ActivateDialogue },
            { ResultFuncsEnum.StartMiniGame,        StartMiniGame },
            { ResultFuncsEnum.PlayMusic,            PlayMusic },
            { ResultFuncsEnum.OpenFile,             OpenFile},
            { ResultFuncsEnum.OpenFact,             OpenFact},
            { ResultFuncsEnum.ChangeParameter,      ChangeParameter},

            { ResultFuncsEnum.TryToStartDialogue,   TryToStartDialogue},
            { ResultFuncsEnum.ActivateObject,       ActivateObject},

            { ResultFuncsEnum.StartBusiness,        StartBusiness},
            { ResultFuncsEnum.GetChoose,            GetChoose},

            { ResultFuncsEnum.AddToContactList,     AddToContactList},
            { ResultFuncsEnum.AddToFlype,           AddToFlype},
            { ResultFuncsEnum.AddToFF,              AddToFF},
            { ResultFuncsEnum.DeleteContact,        DeleteContact},
            { ResultFuncsEnum.DeleteContactFF,      DeleteFF},
            { ResultFuncsEnum.DeactivateBusiness,   DeactivateBusiness},
            { ResultFuncsEnum.ActivateMiniGame,     ActivateMiniGame},
            { ResultFuncsEnum.DeactivateMiniGame,   DeactivateMiniGame},
            { ResultFuncsEnum.DeactivateDialogue,   DeactivateDialogue},
            { ResultFuncsEnum.StartDialogue,        StartDialogue},
            { ResultFuncsEnum.AddChar,              AddChar},
            { ResultFuncsEnum.GoTo,                 GoTo},
            { ResultFuncsEnum.AddTask,              AddTask},
            { ResultFuncsEnum.SayReplic,            SayReplic},
            { ResultFuncsEnum.Fade,                 Fade},
            { ResultFuncsEnum.ChangeLocState,       ChangeLocState},
            { ResultFuncsEnum.Change,               Change},
            { ResultFuncsEnum.Teleport,             Teleport},
            { ResultFuncsEnum.ShowTask,             ShowTask},
            { ResultFuncsEnum.CloseTask,            CloseTask},
            { ResultFuncsEnum.ShowSubtitles,        ShowSubtitles},
            { ResultFuncsEnum.ShowNotification,     ShowNotification},
            { ResultFuncsEnum.ChangeState,          ChangeState},
            { ResultFuncsEnum.DeleteProfile,        DeleteProfile},
            { ResultFuncsEnum.ActivateDevice,       ActivateDevice},
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

        private static void Error(List<string> input)
        {
            Debug.LogError("Необходимо реализовать новый метод для ивентов.");
        }

        private static void ShowSubtitles(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ShowSubtitles, input);
        }

        private static void ChangeState(List<string> input)
        {
            ConnectingAppManager.CharacterManager.GetNPC(input[0]).SetIdModificator(input.Count == 1 ? string.Empty : input[1]);
        }

        private static void ShowNotification(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ShowNotification, input);
        }

        private static void CloseTask(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.CloseTask, input);
        }

        private static void ShowTask(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ShowTask, input);
        }

        private static void ActivateDevice(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateDevice, input);
        }

        private static void AddChar(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.AddChar, input);
        }

        private static void GoTo(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.GoTo, input);
        }

        private static void SayReplic(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.SayReplic, input);
        }

        private static void Fade(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.Fade, input);
        }

        private static void ChangeLocState(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ChangeLocState, input);
        }

        private static void Change(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.Change, input);
        }

        private static void Teleport(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.Teleport, input);
        }

        private static void AddTask(List<string> input)
        {
            CoreController.EventsManager.ActivateTask(input[0]);
        }

        private static void AddToContactList(List<string> input)
        {
            ConnectingAppManager.CharacterManager.AddContact(input[0], Player.ContactType.Phone);
        }

        private static void AddToFlype(List<string> input)
        {
            ConnectingAppManager.CharacterManager.AddContact(input[0], Player.ContactType.Flype);
        }

        private static void AddToFF(List<string> input)
        {
            ConnectingAppManager.CharacterManager.AddContact(input[0], Player.ContactType.FF);
        }

        private static void DeleteFF(List<string> input)
        {
            ConnectingAppManager.CharacterManager.RemoveContact(input[0], Player.ContactType.FF);
        }

        private static void DeleteContact(List<string> input)
        {
            ConnectingAppManager.CharacterManager.RemoveContact(input[0], Player.ContactType.Phone);
        }

        private static void ActivateMiniGame(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateMiniGame, input);
        }

        private static void DeactivateMiniGame(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.DeactivateMiniGame, input);
        }

        private static void TryToStartDialogue(List<string> input)
        {
            FormatDialogue mode = (FormatDialogue)Int32.Parse(input[2]);
            var dialogue = ConnectingAppManager.CharacterManager.GetNPC(input[0]).GetDialog(mode, input[1]);
            if (dialogue != null && !ConnectingAppManager.DialogManager.ActiveDialogs.Contains(dialogue))
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.TryToStartDialogue, input);
        }

        private static void ActivateDialogue(List<string> dialogues)
        {
            foreach (var d in dialogues)
            {
                var dialogue = CoreController.DialogueManager.GetDialogue(d);

                if (string.IsNullOrEmpty(dialogue.BusinessId) || ConnectingAppManager.BusinessManager.GetActualBusinessId().Equals(dialogue.BusinessId))
                {
                    if (dialogue == null)
                    {
                        foreach (var dg in CoreController.DialogueManager.GetDialoguesGroup(d))
                        {
                            var dialog = CoreController.DialogueManager.GetDialogue(d);
                            SaveDialog(dialog);
                        }
                    }
                    else SaveDialog(dialogue);
                }
                else Debug.Log($"Попытка активировать диалог, доступный только в занятии: {dialogue.BusinessId}\n" +
                               $"А сейчас занятие: {ConnectingAppManager.BusinessManager.GetActualBusinessId()}");
            }
        }

        private static void SaveDialog(Dialogue dialogue)
        {
            Dialog dialog = new Dialog(dialogue);
            ConnectingAppManager.CharacterManager.AddDialog(dialog, dialog.Participants.First());
        }

        private static void DeactivateDialogue(List<string> dialogues)
        {
            foreach (var d in dialogues)
            {
                var dialogue = CoreController.DialogueManager.GetDialogue(d);
                if (dialogue != null)
                {
                    Dialog dialog = new Dialog(dialogue);
                    ConnectingAppManager.CharacterManager.RemoveDialog(dialog, dialog.Participants.First());
                }
                else Debug.LogError($"Попытка деактивировать несуществующий диалог с id: \"{d}\"");
            }
        }

        private static void StartDialogue(List<string> input)
        {
            var dialogue = CoreController.DialogueManager.GetDialogue(input[0]);
            Dialog d = new Dialog(dialogue);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.StartDialogue, new List<string>() { d.Participants.First(), d.Id, ((int)d.Format).ToString() });
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
            {
                ConnectingAppManager.BusinessManager.AddAvailableBusiness(b);
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.ActivateBusiness, businesses);
            }
        }

        private static void DeactivateBusiness(List<string> businesses)
        {
            foreach (var b in businesses)
            {
                ConnectingAppManager.BusinessManager.RemoveAvailableBusiness(b);
                TriangleManager.InvokeResultFuncs(ResultFuncsEnum.DeactivateBusiness, businesses);
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
            int slotsCount = 0;
            slotsCount = input.Count == 0 ? ConnectingAppManager.BusinessManager.GetCountOfSlotsForActualBusinessInfo() : int.Parse(input.First());
            ShowCoreAndConnectingAppEntities.Instance.Date = CoreController.TimeModule.MoveSlot(slotsCount);
            CoreController.ChangeHealth(-slotsCount);
            switch (CoreController.Health)
            {
                case 0:
                    Debug.LogError("Game over! Because player health is zero.");
                    break;

                case 5:
                case 7:
                    Debug.Log($"Info: The player health are equal: {CoreController.Health}");
                    break;

                default:
                    break;
            }
        }

        private static void StartMiniGame(List<string> input)
        {
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.StartMiniGame, input);
        }

        private static void ChangeParameter(List<string> input)
        {
            var parameter = parameters[input[0]];
            string dialogId = input[1];
            int init = int.Parse(input[2]);
            CoreController.DialogueManager.ChangeParameter(parameter, input[1], int.Parse(input[2]));
        }

        private static void OpenFact(List<string> input)
        {
            ConnectingAppManager.CharacterManager.GetNPC(input[0]).AddFact(input[1]);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.OpenFact, input);
        }

        private static void DeleteProfile(List<string> input)
        {
            ConnectingAppManager.CharacterManager.DeleteFile(input[0]);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.DeleteProfile, input);
        }

        private static void OpenFile(List<string> input)
        {
            ConnectingAppManager.CharacterManager.CreateFile(input[0]);
            TriangleManager.InvokeResultFuncs(ResultFuncsEnum.OpenFile, input);
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