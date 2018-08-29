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
        private static readonly string PATH_TO_LOCALIZATION_FILES = string.Concat(Application.dataPath, "/Localizations/");
        private static readonly string PATH_TO_CONFIG_FILES = string.Concat(Application.dataPath, "/ConfigFiles/");

        public static string GetPathToLocalizationFiles(ELanguage eLanguage)
        {
            return string.Concat(PATH_TO_LOCALIZATION_FILES, eLanguage == ELanguage.en ? "EN/" : "RU/");
        }

        public static string GetPathToConfigFiles()
        {
            return PATH_TO_CONFIG_FILES;
        }
    }
}
