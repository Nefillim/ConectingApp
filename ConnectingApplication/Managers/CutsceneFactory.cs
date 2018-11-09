using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ConectingApp.ConnectingApplication.Managers
{
    public class CutsceneFactory
    {
        private static readonly Dictionary<string, Action> cutscenes = new Dictionary<string, Action>()
        {

        };


        [Obsolete("Don't use outside the ConnectingApp.")]
        public CutsceneFactory()
        {

        }


        public void StartCutscene(string cutsceneId)
        {
            cutscenes[cutsceneId].Invoke();
        }
    }
}
