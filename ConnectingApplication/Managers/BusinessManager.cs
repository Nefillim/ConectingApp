using Core.Business;
using Startuper.Devices.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Managers
{
    public class BusinessManager
    {
        private List<string> impossibleBusinesses;
        private List<string> availableBusiness;
        private BusinessInfo actualBusinessInfo;


        public event Action<List<string>> ChangedBusinesses;
        public event Action<string, string> NewBusiness;

        [Obsolete("Don't use outside the ConnectingApp.")]
        public BusinessManager()
        {
            availableBusiness = new List<string>();
            impossibleBusinesses = new List<string>();
        }


        private void RemoveOldAndImpossibleBusinesses()
        {
            // Получаем все доступные занятия без проверки дат.
            var objects = Core.CoreController.BusinessManager.GetBusinesses(availableBusiness, true);
            var currentDate = ConnectingAppManager.Date;
            var j = 0;
            var changedBusinesses = new List<string>();

            // Если все даты возможного проведения занятия < текущей, значит оно больше никогда не будет доступно.
            foreach (var i in objects)
            {
                var dates = CalendarMenu.ParseDates(i.Condition);
                var canHappen = false;
                j = 0;

                while (!canHappen && j < dates.Count)
                {
                    canHappen = currentDate <= dates[j].Day * 10 + dates[j].Slot;
                    ++j;
                }

                if (!canHappen)
                {
                    availableBusiness.Remove(i.BusinessId);
                    impossibleBusinesses.Add(i.BusinessId);
                    changedBusinesses.Add(i.BusinessId);
                }
            }
            ChangedBusinesses.Invoke(changedBusinesses);
        }


        public IList<Business> GetBusiness(bool forCalendar)
        {
            var businesses = availableBusiness;
            if (forCalendar)
            {
                businesses = new List<string>();
                businesses.AddRange(availableBusiness);
                businesses.AddRange(impossibleBusinesses);
            }
            return Core.CoreController.BusinessManager.GetBusinesses(businesses, forCalendar).AsReadOnly();
        }

        public BusinessInfo GetBusinessInfo(string businessId)
        {
            RemoveOldAndImpossibleBusinesses();
            if (actualBusinessInfo == null || !businessId.Equals(actualBusinessInfo.BusinessId))
            {
                var newBusinessInfo = Core.CoreController.BusinessManager.GetBusinessInfo(businessId);
                if (actualBusinessInfo != null)
                    NewBusiness.Invoke(actualBusinessInfo.BusinessId, newBusinessInfo.BusinessId);
                actualBusinessInfo = newBusinessInfo;
            }
            return actualBusinessInfo;
        }

        public void AddAvailableBusiness(string business)
        {
            availableBusiness.Add(business);
        }

        public void RemoveAvailableBusiness(string business)
        {
            availableBusiness.Remove(business);
        }

        public void SetFlagsWhenBusinessStart(BusinessInfo business)
        {
            ConnectingAppManager.FlagManager.SetFlags(business.ResultFlags);
        }

        public int GetCountOfSlotsForActualBusinessInfo()
        {
            return actualBusinessInfo.SlotsCount;
        }

        public string GetTagForActualBusiness()
        {
            return actualBusinessInfo.BusinessId;
        }
    }
}
