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
        private BusinessInfo actualBusinessInfo;


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
            if (actualBusinessInfo == null || !businessId.Equals(actualBusinessInfo.BusinessId))
                actualBusinessInfo = Core.CoreController.BusinessManager.GetBusinessInfo(businessId);
            return actualBusinessInfo;
        }

        public void AddAvailableBusiness(string business)
        {
            availableBusiness.Add(business);
        }

        public void SetFlagsWhenBusinessStart(BusinessInfo business)
        {
            ConnectingAppManager.FlagManager.SetFlags(business.ResultFlags);
        }

        public int GetCountOfSlotsForActualBusinessInfo()
        {
            return actualBusinessInfo.SlotsCount;
        }
    }
}
