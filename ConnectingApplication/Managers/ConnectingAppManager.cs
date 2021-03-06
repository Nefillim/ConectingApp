﻿using Assets.ConectingApp.ConnectingApplication.Managers;
using Assets.Scripts.Helpers;
using Core;
using Core.TimeMachine;
using System;
using UnityEngine;
using static Core.TimeMachine.TimeModule;

namespace ConnectingApplication.Managers
{
    public static class ConnectingAppManager
    {
        private static bool _saveMode = false;


        public static BusinessManager BusinessManager { get; private set; }
        public static CharacterManager CharacterManager { get; private set; }
        public static DialogManager DialogManager { get; private set; }
        public static DownloadManager DownloadManager { get; private set; }
        public static EventResultsManager EventResultsManager { get; private set; }
        public static FlagManager FlagManager { get; private set; }
        public static SaveManager SaveManager { get; private set; }
        public static CutsceneFactory CutsceneFactory { get; private set; }
        public static int Date { get { return CoreController.TimeModule.GetDate(); } }
        public static float Balance { get { return CoreController.Balance; } }
        public static int Health { get { return CoreController.Health; } }
        [Obsolete("Don't use outside the DownloadManager.")]
        public static bool SaveMode { get; set; }


        private static void ExceptionListener(string message, EMessageType messageType)
        {
            if (messageType == EMessageType.Error)
                Debug.LogError(message);
            else
                Debug.Log($"{messageType.ToString()}:\n {message}");
        }


        public static bool StartApp()
        {
            #pragma warning disable CS0618 // Не придумал другой защиты от создания новых экземпляров классов.
            BusinessManager = new BusinessManager();
            CharacterManager = new CharacterManager();
            DialogManager = new DialogManager();
            DownloadManager = new DownloadManager();
            EventResultsManager = new EventResultsManager();
            CutsceneFactory = new CutsceneFactory();
            FlagManager = new FlagManager();
            SaveManager = new SaveManager();
            #pragma warning restore CS0618 // Не придумал другой защиты от создания новых экземпляров классов.

            CoreController.ResultMethod += EventResultsManager.CoreEventsResult;
            CoreController.ExceptionMethod += ExceptionListener;

            return true;
        }

        public static bool StartCore(string pathToConfigFiles, string stepName)
        {
            var parseResult = CoreController.StartCore(pathToConfigFiles, stepName);
            ShowCoreAndConnectingAppEntities.Instance.Date = Date;

            return parseResult;
        }

        public static bool ParseAndCheckCondition(string condition)
        {
            return CoreController.ConditionParser.ParseAndCheckCondition(condition);
        }
    }
}