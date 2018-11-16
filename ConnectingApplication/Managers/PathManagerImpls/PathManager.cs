using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ConectingApp.ConnectingApplication.Managers.PathManagerImpls
{
    public static class PathManager
    {
        private static readonly string PATH_TO_LOCALIZATION_FILES = "/Localizations/";
        private static readonly string PATH_TO_CONFIG_FILES = "/ConfigFiles/";
        private static readonly string PATH_TO_MINI_GAME_FILES = "/MiniGames/";
		private static readonly string PATH_TO_SAVE = "/GameProgress/";
        private static readonly string PATH_TO_DATA = Application.streamingAssetsPath;


        public static string GetPathToLocalizationFiles(LocalizationType localizationType)
        {
            return string.Concat(PATH_TO_DATA, PATH_TO_LOCALIZATION_FILES, localizationType.ToString(), "/");
        }

        public static string GetPathToConfigFiles()
        {
            return string.Concat(PATH_TO_DATA, PATH_TO_CONFIG_FILES);
        }

        public static string GetPathToMiniGameFilesFolder()
        {
            return string.Concat(GetPathToConfigFiles(), PATH_TO_MINI_GAME_FILES);
        }

		public static string GetPathToSaveFilesDirectory()
		{
			return string.Concat(PATH_TO_DATA, PATH_TO_SAVE);
		}
    }
}
