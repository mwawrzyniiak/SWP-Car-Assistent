using SWPCarAssistent.Core.Common.Interfaces;
using System;
using System.Linq;
using SWPCarAssistent.Core.Common.Entities;

namespace SWPCarAssistent.Infrastructure.Repositories
{
    public class CarRepository : IExampleRepository
    {
        public void UpdateRadioStation()
        {
            using (var db = new CarContext())
            {
                var test =db.Radio.Find(1);
            }
        }
    }
}
