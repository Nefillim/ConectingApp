using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ConectingApp.ConnectingApplication.Managers
{
    public static class DefaultValues
    {
        public static readonly string PLAYER_ID = "charPlayer";
        public static readonly KeyCode PHONE_ENABLE_KEY = KeyCode.Q;
        public static readonly KeyCode TABLET_ENABLE_KEY = KeyCode.E;
        public static readonly KeyCode INTERACTION_KEY = KeyCode.F;
        public static readonly KeyCode OPEN_TASKS_KEY = KeyCode.Tab;
        public static readonly int WAITING_SECONDS_BEFORE_CALL = 10;
        public static readonly int SEARCHING_DISTANCE = 10;
        public static readonly int MIN_DISTANCE_FOR_INTERACTION = 5;
    }
}
