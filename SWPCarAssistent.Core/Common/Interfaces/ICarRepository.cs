using SWPCarAssistent.Core.Common.Entities;
using System.Collections.Generic;

namespace SWPCarAssistent.Core.Common.Interfaces
{
    public interface ICarRepository
    {
        void AddNewContact(EntityContacts Contacts);
        void AddNewRadioStation(EntityRadio Radio);
        void ChangeStartupParams(EntityStartupParams startupParams);
        List<EntityRadio> GetAllContacts();
        EntityContacts GetContact(string name);
        EntityRadio GetRadioStation(string radioName);
        EntityStartupParams GetStartupParams();
    }
}