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
        private Queue<Flag> saveQueue;
        private static int iteratorPosition;


        [Obsolete("Don't use outside the ConnectingApp.")]
        public DownloadManager()
        {
            saveQueue = new Queue<Flag>();
        }

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
