using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public static class DownloadManager
    {
        private static Queue<Flag> saveQueue = new Queue<Flag>();
        private static int iteratorPosition;

        public static void EnqueueFlag(Flag flag)
        {
            saveQueue.Enqueue(flag);
        }

        public static void SetNewIteratorPosition()
        {
            iteratorPosition = saveQueue.Count;
        }
    }
}
