using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public static class GameManager
    {
        public static bool StartCore(string pathToConfigFiles, string stepName)
        {
            CoreController.ResultMethod += EventResultsManager.CoreEventsResult;
            CoreController.ExceptionMethod += ExceptionListener;
            var parseResult = CoreController.StartCore(pathToConfigFiles, stepName);
            return parseResult;
        }

        private static void ExceptionListener(string message)
        {
            Debug.LogError(message);
        }
    }
}
