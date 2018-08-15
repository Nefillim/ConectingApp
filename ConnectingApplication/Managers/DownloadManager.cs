using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class DownloadManager 
    {
        private Queue<Flag> saveQueue = new Queue<Flag>();
        private static int iteratorPosition;


        public void EnqueueFlag(Flag flag)
        {
            saveQueue.Enqueue(flag);
        }

        public void SetNewIteratorPosition()
        {
            iteratorPosition = saveQueue.Count;
        }

		public void Download()
		{

		}

		public void Save()
		{

		}
    }
}
