using Core.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class BusinessManager 
    {
        private List<string> availableBusiness;

        
        [Obsolete("Don't use outside the ConnectingApp.")]
        public BusinessManager()
        {
            availableBusiness = new List<string>();
        }


        public IList<Business> GetBusiness(bool forCalendar)
        {
            return Core.CoreController.BusinessManager.GetBusinesses(availableBusiness, forCalendar).AsReadOnly();
        }

        public BusinessInfo GetBusinessInfo(string businessId)
        {
            return Core.CoreController.BusinessManager.GetBusinessInfo(businessId);
        }

        public void AddAvailableBusiness(string business)
        {
            availableBusiness.Add(business);
        }

        public void SetFlagsWhenBusinessStart(BusinessInfo business)
        {
            business.SetFlags();
        }
    }
}
