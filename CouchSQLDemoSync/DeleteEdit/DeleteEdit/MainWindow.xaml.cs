using CouchSQLSync_v4.Database;
using CouchSQLSync_v4.Database.TableSpecificData.UsableData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DeleteEdit
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableCollection<string> dataList;
		private ObservableCollection<Ticket> ticketList;
		private ActiveTicket ticket;
		private Helper _helper;

		#region Public Observables
		public ObservableCollection<string> DataList
		{
			get { return this.dataList; }
			set
			{
				if (value != this.dataList)
				{
					this.dataList = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ObservableCollection<Ticket> TicketList
		{
			get { return this.ticketList; }
			set
			{
				if (value != this.ticketList)
				{
					this.ticketList = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ActiveTicket Ticket
		{
			get { return this.ticket; }
			set
			{
				if (value != this.ticket)
				{
					this.ticket = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Helper _Helper
		{
			get { return this._helper; }
			set
			{
				if (value != this._helper)
				{
					this._helper = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;

			using (DatabaseContext db = new DatabaseContext())
			{
				this.DataList = new ObservableCollection<string>();
				this.TicketList = new ObservableCollection<Ticket>();
				this.Ticket = new ActiveTicket();
				this._Helper = new Helper();

				ObservableCollection<string> tempDataList = new ObservableCollection<string>();

				List<Ticket> tempTicketList = db.Tickets.ToList();
				foreach (Ticket tempTicket in tempTicketList)
				{
					this.TicketList.Add(tempTicket);
					tempDataList.Add(tempTicket._id);

					ActiveTicket tempActiveTicket = new ActiveTicket();
					tempActiveTicket.Active = tempTicket;
					this.Ticket = tempActiveTicket;
				}
				this.DataList = tempDataList;
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void ComboBox_SelectionChanged_Type(object sender, SelectionChangedEventArgs e)
		{
			ComboBox temp = (ComboBox)sender;
			ComboBoxItem _selection = (ComboBoxItem)temp.SelectedItem;

			if (_selection.Content != null)
			{
				if (_selection.Content.ToString().Equals("Edit"))
				{
					this._Helper._Readonly = "false";
				}
				if (_selection.Content.ToString().Equals("Delete"))
				{
					this._Helper._Readonly = "true";
				}
			}
		}

		private void ComboBox_SelectionChanged_Ticket(object sender, SelectionChangedEventArgs e)
		{
			ComboBox temp = (ComboBox)sender;
			string _selection = (string)temp.SelectedItem;

			foreach (Ticket tempTicket in this.TicketList)
			{
				if (tempTicket._id.Equals(_selection))
				{
					this.Ticket.Active = tempTicket;
				}
			}
		}
	}

	public class ActiveTicket : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private Ticket active;
		public Ticket Active
		{
			get { return this.active; }
			set
			{
				if (value != this.active)
				{
					this.active = value;
					NotifyPropertyChanged();
				}
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class Helper : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _readonly;
		public string _Readonly
		{
			get { return this._readonly; }
			set
			{
				if (value != this._readonly)
				{
					this._readonly = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Helper()
		{
			this._Readonly = "false";
		}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public enum Selection
	{
		edit = 0,
		delete = 1
	}
}
