using AddData.Database.TableSpecificData.UsableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddData.Factory
{
    public class DataFactory
    {
        public PouchData GetData(string type)
        {
            if (type == "Ticket")
            {
                Ticket temp = new Ticket();

                temp.ActivityCount = 1;
                temp.City = "Salzburg";
                temp.Country = "A";
                temp.Description = "Added-Offline-Sync-Up";
                temp.EndDate = "28.02.2018 08=00=00";
                temp.IsFixedTimeFrame = false;
                temp.LatestCompletionDate = "";
                temp.LocationAddress1 = "StraßenName Hausnummer";
                temp.LocationAddress2 = "";
                temp.LocationName1 = "Firmenname";
                temp.LocationName2 = "";
                temp.ServiceCallId = "Firma/TicketID";
                temp.ServiceCallLineItemCount = 1;
                temp.StartDate = "28.02.2018 08=00=00";
                temp.PersId = "PersonenIdentifizierung";
                temp.Title = "Firma";
                temp.ZipCode = "5412";
                temp.TicketId = "2468";

                temp._id = Guid.NewGuid().ToString();
                temp._rev = "1-" + Guid.NewGuid().ToString();

                return temp;
            }

            return null;
        }
    }
}
