using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ConectingApp.ConnectingApplication.Managers
{
    public static class PathManager
    {
        public static readonly string PATH_TO_CONFIG_FILES = string.Concat(Application.dataPath, "/ConfigFiles");

        public static readonly string PATH_TO_LOCALIZATION_FILES = string.Concat(Application.dataPath, "/LocalizationFiles");
    }
}
