using AddData.Database;
using AddData.Database.TableSpecificData.UsableData;
using AddData.Database.UniversalData;
using AddData.Factory;
using AddData.SyncMiddleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AddData
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int DataAmount { get; set; }

        private DataFactory Factory;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            this.Factory = new DataFactory();
            
            this.Init();
        }

        private void Init()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (db.Info.ToList().Count == 0)
                {
                    VendorInfo tempVendor = new VendorInfo();
                    tempVendor.name = "FH-Salzburg";

					ConnectorInfo newConnector = new ConnectorInfo();
					newConnector.Vendor = tempVendor;
					newConnector.Version = "0.1.4";
					newConnector.UpdateSeq = "relpace-with-latest-update-seq";
					newConnector.PeerUUID = Guid.NewGuid().ToString();
					
					db.Info.Add(newConnector);
					db.SaveChanges();
                }
            }
        }

        private void AddData(object sender, RoutedEventArgs e)
        {
            if (DataAmount > 0)
            {
                List<Ticket> newTickets = new List<Ticket>();
                for (int i = 0; i < DataAmount; i++)
                {
                    Ticket newTicket = (Ticket)Factory.GetData("Ticket");
                    newTickets.Add(newTicket);

					using(MessageControllerHelper helper = new MessageControllerHelper())
					{
						helper.AddNewChange(newTicket, "Tickets");
					}
                }

                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Tickets.AddRange(newTickets);
                    
                    db.SaveChanges();
                }
            }
        }
    }
}
