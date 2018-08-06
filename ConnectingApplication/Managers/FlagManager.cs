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
	public static class FlagManager
	{
		public static int GetFlag(string flagName)
		{
			return Core.CoreController.FlagsManager.Get(flagName);
		}

		public static bool SetFlag(string flagName, int value)
		{
			if (Core.CoreController.FlagsManager.Set(flagName, value))
			{
				Flag flag;
				flag.name = flagName;
				flag.value = value;
				DownloadManager.EnqueueFlag(flag);
				return true;
			}
			return false;
		}


	}
}
