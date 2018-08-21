using Core;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public static class ConnectingAppManager
    {
        public static readonly string PLAYER_ID = "charPlayer";

        public static BusinessManager BusinessManager { get; private set; }
        public static CharacterManager CharacterManager { get; private set; }
        public static DialogManager DialogManager { get; private set; }
        public static DownloadManager DownloadManager { get; private set; }
        public static EventResultsManager EventResultsManager { get; private set; }
        public static FlagManager FlagManager { get; private set; }
        public static SaveManager SaveManager { get; private set; }


        private static void ExceptionListener(string message)
        {
            Debug.LogError(message);
        }


        public static bool StartAppAndCore(string pathToConfigFiles, string stepName)
        {
#pragma warning disable CS0618 // Не придумал другой защиты от создания новых экземпляров классов.
            BusinessManager = new BusinessManager();
            CharacterManager = new CharacterManager();
            DialogManager = new DialogManager();
            DownloadManager = new DownloadManager();
            EventResultsManager = new EventResultsManager();
            FlagManager = new FlagManager();
            SaveManager = new SaveManager();
#pragma warning restore CS0618 // Не придумал другой защиты от создания новых экземпляров классов.

            CoreController.ResultMethod += EventResultsManager.CoreEventsResult;
            CoreController.ExceptionMethod += ExceptionListener;
            var parseResult = CoreController.StartCore(pathToConfigFiles, stepName);
            return parseResult;
        }
    }
}
