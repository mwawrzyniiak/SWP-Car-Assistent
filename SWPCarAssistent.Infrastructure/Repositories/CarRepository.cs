using SWPCarAssistent.Core.Common.Entities;
using SWPCarAssistent.Core.Common.Interfaces;
using SWPCarAssistent.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;

namespace SWPCarAssistent.Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        public CarRepository() { }
        public void AddNewRadioStation(Radio Radio)
        {
            using (var context = new CarContext())
            {
                context.Radio.Add(Radio);
                context.SaveChanges();
            }
        }

        public Radio GetRadioStation(string radioName)
        {
            using (var context = new CarContext())
            {
                return context.Radio?.Where(b => b.RadioName.Contains(radioName)).FirstOrDefault();
            }
        }
        public List<Radio> GetAllRadios()
        {
            using (var context = new CarContext())
            {
                return context.Radio.ToList();
            }
        }

        public void AddNewContact(Contacts Contacts)
        {
            using (var context = new CarContext())
            {
                context.Contacts?.Add(Contacts);
                context.SaveChanges();
            }
        }
        public List<Contacts> GetAllContacts()
        {
            using (var context = new CarContext())
            {
                return context.Contacts.ToList();
            }
        }
        public Contacts GetContact(string name)
        {
            using (var context = new CarContext())
            {
                return context.Contacts?.Where(b => b.FullName.Contains(name)).FirstOrDefault();
            }
        }
        public void ChangeStartupParams(StartupParams startupParams)
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
        public StartupParams GetStartupParams()
        {
            using (var db = new CarContext())
            {
                return db.StartupParams.Where(b => b.Id.Equals(1)).First();
            }

        }
    }
}