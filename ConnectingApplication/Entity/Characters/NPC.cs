using ConnectingApplication.Entity;
using ConnectingApplication.Entity.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Characters
{
	public class NPC : Character
	{
		public void AddDialog(Dialog d)
		{
			AvailableDialogs.Add(d);
		}
		public List<Dialog> GetDialogs()
		{
			return AvailableDialogs;
		}
	}
}
