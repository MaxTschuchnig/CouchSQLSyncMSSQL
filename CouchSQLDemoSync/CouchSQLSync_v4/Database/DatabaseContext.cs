namespace CouchSQLSync_v4.Database
{
	using CouchSQLSync_v4.Database.TableSpecificData.Changes;
	using CouchSQLSync_v4.Database.TableSpecificData.ReplicationLog;
	using CouchSQLSync_v4.Database.TableSpecificData.UsableData;
	using CouchSQLSync_v4.Database.UniversalData;
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