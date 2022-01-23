using SWPCarAssistent.Core.Common.Entities;
using System.Collections.Generic;

namespace SWPCarAssistent.Core.Common.Interfaces
{
    public interface ICarRepository
    {
        void AddNewContact(Contacts Contacts);
        void AddNewRadioStation(Radio Radio);
        void ChangeStartupParams(StartupParams startupParams);
        List<Contacts> GetAllContacts();
        Contacts GetContact(string name);
        Radio GetRadioStation(string radioName);
        StartupParams GetStartupParams();
    }
}