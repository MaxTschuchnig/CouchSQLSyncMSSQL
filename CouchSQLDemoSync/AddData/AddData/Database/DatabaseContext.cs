namespace AddData.Database
{
	using AddData.Database.TableSpecificData.Changes;
	using AddData.Database.TableSpecificData.ReplicationLog;
	using AddData.Database.TableSpecificData.UsableData;
	using AddData.Database.UniversalData;
	using System.Data.Entity;

	public class DatabaseContext : DbContext
	{
		public DatabaseContext()
			: base("Server=localhost\\SQLEXPRESS;Database=CouchDBSync_v1.4;Trusted_Connection=True;MultipleActiveResultSets=True;") { }

		public virtual DbSet<ConnectorInfo>		Info				{ get; set; }
		public virtual DbSet<ReplicationLog>	ReplicationLog		{ get; set; }
		public virtual DbSet<Change>			Changes				{ get; set; }

		// Add all predefined Tables here! If not, the framework cannot find them!
		public string[]							ExistingTables =	{ "Tickets" };
		public virtual DbSet<Ticket>			Tickets				{ get; set; }
	}
}