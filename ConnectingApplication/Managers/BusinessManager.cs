using Assets.Scripts.Helpers;
using Core;
using Core.Business;
using Startuper.Devices.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConnectingApplication.Managers
{
    public class BusinessManager
    {
        private List<string> impossibleBusinesses;
        private List<string> availableBusiness;
        private BusinessInfo actualBusinessInfo;
        private System.Random random;


        public event Action<List<string>> ChangedBusinesses;
        public event Action<string, string> NewBusiness;

        [Obsolete("Don't use outside the ConnectingApp.")]
        public BusinessManager()
        {
            availableBusiness = new List<string>();
            impossibleBusinesses = new List<string>();

            ShowCoreAndConnectingAppEntities.Instance.AvailableBusiness = availableBusiness;
            ShowCoreAndConnectingAppEntities.Instance.ImpossibleBusinesses = impossibleBusinesses;

            random = new System.Random();
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
            ChangedBusinesses?.Invoke(changedBusinesses);
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
            return CoreController.BusinessManager.GetBusinesses(businesses, forCalendar).AsReadOnly();
        }

        public BusinessInfo GetBusinessInfo(string businessId)
        {
            RemoveOldAndImpossibleBusinesses();
            if (actualBusinessInfo == null || !businessId.Equals(actualBusinessInfo.BusinessId))
            {
                var newBusinessInfo = Core.CoreController.BusinessManager.GetBusinessInfo(businessId);
                if (actualBusinessInfo != null)
                {
                    NewBusiness.Invoke(actualBusinessInfo.BusinessId, newBusinessInfo.BusinessId);
                    if (!actualBusinessInfo.Location.Equals(newBusinessInfo.Location))
                        ConnectingAppManager.EventResultsManager.CoreEventsResult("ChangeBalance", 
                                                                                  new List<string> { (-(float)Math.Round(random.NextDouble() * 4 + 3, 2)).ToString() });
                }
                actualBusinessInfo = newBusinessInfo;
                ShowCoreAndConnectingAppEntities.Instance.ActualBusinessInfo = actualBusinessInfo;
            }
            return actualBusinessInfo;
        }

        public void AddAvailableBusiness(string business)
        {
            if (!availableBusiness.Contains(business))
                availableBusiness.Add(business);
            else
                Debug.LogError($"Попытка добавить занятие которое уже есть: \"{business}\"");
        }

        public void RemoveAvailableBusiness(string business)
        {
            if (availableBusiness.Contains(business))
                availableBusiness.Remove(business);
            else if (impossibleBusinesses.Contains(business))
                impossibleBusinesses.Remove(business);
            else Debug.LogError($"Попытка деактивировать неактивированное занятие с id: {business}");
        }

        public void SetFlagsWhenBusinessStart(BusinessInfo business)
        {
            ConnectingAppManager.FlagManager.SetFlags(business.ResultFlags);
        }

        public int GetCountOfSlotsForActualBusinessInfo()
        {
            return actualBusinessInfo.SlotsCount;
        }

        public string GetActualBusinessId()
        {
            return actualBusinessInfo.BusinessId;
        }
    }
}
