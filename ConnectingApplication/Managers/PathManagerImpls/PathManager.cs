using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ConectingApp.ConnectingApplication.Managers.PathManagerImpls
{
    public static class PathManager
    {
        private static readonly string PATH_TO_LOCALIZATION_FILES = "/Localizations/";
        private static readonly string PATH_TO_REPLICS_FILES = "/Replics/";
        private static readonly string PATH_TO_CHARACTER_FILES = "/CharacterFiles/";
        private static readonly string PATH_TO_CONFIG_FILES = "/ConfigFiles/";
        private static readonly string PATH_TO_DATA = Application.streamingAssetsPath;

        public static string GetPathToLocalizationReplicsFiles(ELanguage eLanguage)
        {
            return string.Concat(PATH_TO_DATA, PATH_TO_LOCALIZATION_FILES, eLanguage.ToString(), PATH_TO_REPLICS_FILES);
        }

        public static string GetPathToLocalizationCharacterFiles(ELanguage eLanguage)
        {
            return string.Concat(PATH_TO_DATA, PATH_TO_LOCALIZATION_FILES, eLanguage.ToString(), PATH_TO_CHARACTER_FILES);
        }

        public static string GetPathToConfigFiles()
        {
            return string.Concat(PATH_TO_DATA, PATH_TO_CONFIG_FILES);
        }
    }
}
