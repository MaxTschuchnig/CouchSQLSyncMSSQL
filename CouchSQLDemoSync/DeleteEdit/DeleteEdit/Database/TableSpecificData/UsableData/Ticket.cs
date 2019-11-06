namespace CouchSQLSync_v4.Database.TableSpecificData.UsableData
{
	public class Ticket : PouchData
	{
		public int		ActivityCount				{ get; set; }

		public string	City						{ get; set; }
		public string	Country						{ get; set; }

		public string	Description					{ get; set; }
		public string	EndDate						{ get; set; }
		public bool		IsFixedTimeFrame			{ get; set; }
		public string	LatestCompletionDate		{ get; set; }

		public string	LocationAddress1			{ get; set; }
		public string	LocationAddress2			{ get; set; }
		public string	LocationName1				{ get; set; }
		public string	LocationName2				{ get; set; }

		public string	ServiceCallId				{ get; set; }
		public int		ServiceCallLineItemCount	{ get; set; }
		public string	StartDate					{ get; set; }

		public string	PersId						{ get; set; }

		public string	Title						{ get; set; }
		public string	ZipCode						{ get; set; }

		public string	TicketId					{ get; set; }
	}
}
