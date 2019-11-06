using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CouchSQLSync_v4.Database.TableSpecificData.Changes
{
	public class Change
	{
		[Key]
		public string									seq				{ get; set; }
		public string									DatabaseName	{ get; set; }

		public string									id				{ get; set; }
		public bool										deleted			{ get; set; }

		public virtual ICollection<ChangeStringHelper>	changes			{ get; set; }

		public Change()
		{
			this.changes = new List<ChangeStringHelper>();
		}
	}
}