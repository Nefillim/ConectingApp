using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
	public struct Flag 
    {
		public string name;
		public int value;
	}

	public class FlagManager
	{
        [Obsolete("Don't use outside the ConnectingApp.")]
        public FlagManager()
        {

        }

		public int GetFlag(string flagName)
		{
			return Core.CoreController.FlagsManager.GetValue(flagName);
		}

		public bool SetFlag(string flagName, int value)
		{
			if (Core.CoreController.FlagsManager.Set(flagName, value))
			{
				Flag flag;
				flag.name = flagName;
				flag.value = value;
                ConnectingAppManager.DownloadManager.EnqueueFlag(flag);
				return true;
			}
			return false;
		}

        public bool SetFlags(Dictionary<string, int> flags)
        {
            foreach (var item in flags)
            {
                SetFlag(item.Key, item.Value);
            }
            return true;
        }

		public bool CompareFlag(string flagname, int value)
		{
			return Core.CoreController.FlagsManager.GetValue(flagname) == value;
		}

	}
}
