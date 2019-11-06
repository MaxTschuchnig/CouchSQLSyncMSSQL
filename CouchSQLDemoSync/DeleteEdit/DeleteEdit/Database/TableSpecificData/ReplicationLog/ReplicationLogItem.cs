using System.ComponentModel.DataAnnotations;

namespace CouchSQLSync_v4.Database.TableSpecificData.ReplicationLog
{
	public class ReplicationLogItem
	{
		[Key]
		public int Id { get; set; }

		public string last_seq { get; set; }
		public string session_id { get; set; }
	}
}
