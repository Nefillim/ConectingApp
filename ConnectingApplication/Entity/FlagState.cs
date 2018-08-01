using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Entity
{
	public class FlagState
	{
		public string name;
		public bool state;

		public FlagState(string _name, bool _state)
		{
			name = _name;
			state = _state;
		}
	}
}
