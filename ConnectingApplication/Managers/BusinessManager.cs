using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class BusinessManager 
    {
        private List<string> AvailableBusiness;

        public BusinessManager() : base()
        {
            AvailableBusiness = new List<string>();
        }

        public List<Core.Business.Business> GetBusiness(bool forCalendar)
        {
            List<Core.Business.Business> active = new List<Core.Business.Business>();
            active.AddRange(Core.CoreController.BusinessManager.GetBusinesses(AvailableBusiness, forCalendar));
            return active;
        }

        public Core.Business.BusinessInfo GetBusinessInfo(string businessId)
        {
            return Core.CoreController.BusinessManager.GetBusinessInfo(businessId);
        }
    }
}
