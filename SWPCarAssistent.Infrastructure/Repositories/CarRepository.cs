using SWPCarAssistent.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SWPCarAssistent.Core.Common.Entities;
using System.Data.Entity;

namespace SWPCarAssistent.Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        public void AddNewRadioStation(EntityRadio Radio)
        {
            using (var context = new CarContext())
            {
                context.Radio.Add(Radio);
                context.SaveChanges();
            }
        }

        public EntityRadio GetRadioStation(string radioName)
        {
            using (var context = new CarContext())
            {
                return context.Radio?.Where(b => b.RadioName.Contains(radioName)).FirstOrDefault();
            }
        }

        public void AddNewContact(EntityContacts Contacts)
        {
            using (var context = new CarContext())
            {
                context.Contacts?.Add(Contacts);
                context.SaveChanges();
            }
        }
        public List<EntityRadio> GetAllContacts()
        {
            using (var context = new CarContext())
            {
                return context.Radio.ToList();
            }
        }
        public EntityContacts GetContact(string name)
        {
            using (var context = new CarContext())
            {
                return context.Contacts?.Where(b => b.FullName.Contains(name)).FirstOrDefault();
            }
        }
        public void ChangeStartupParams(EntityStartupParams startupParams)
        {
            using (var db = new CarContext())
            {
                var result = db.StartupParams?.FirstOrDefault();
                if (result == null) return;
                result.Radio = startupParams.Radio;
                result.AirConditioning = startupParams.AirConditioning;
                result.CarWindows = startupParams.CarWindows;
                result.Heating = startupParams.Heating;
                result.Lights = startupParams.Lights;
                result.Wipers = startupParams.Wipers;
                db.SaveChanges();
            }
        }
        public EntityStartupParams GetStartupParams()
        {
            using (var db = new CarContext())
            {
                return db.StartupParams?.FirstOrDefault();
            }

        }
    }
}