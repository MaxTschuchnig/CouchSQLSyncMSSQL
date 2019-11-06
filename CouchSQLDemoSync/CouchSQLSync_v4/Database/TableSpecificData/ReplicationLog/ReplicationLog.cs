using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CouchSQLSync_v4.Database.TableSpecificData.ReplicationLog
{
	public class ReplicationLog
	{
		public string	DatabaseName	{ get; set; }
		public string	DocumentId		{ get; set; }

		[Key]
		public int		_id				{ get; set; }
		public string	_rev			{ get; set; }
		
		public virtual ICollection<ReplicationLogItem> history { get; set; }

		public string	replicator		{ get; set; }
		public int		version			{ get; set; }
		public int		last_seq		{ get; set; }

		public ReplicationLog()
		{
			this.history = new List<ReplicationLogItem>();
		}
	}
}
