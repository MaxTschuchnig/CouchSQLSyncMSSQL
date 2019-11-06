using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CouchSQLSync_v4.Database.TableSpecificData.UsableData
{
	public class PouchData
	{
		public string	_id		{ get; set; }
		public bool _deleted	{ get; set; }
		[Key]
		public string	_rev	{ get; set; }

		public PouchData() { }
		public PouchData(string id, string rev)
		{
			this._id = id;
			this._rev = rev;
		}
		public PouchData(string id, string rev, bool deleted)
		{
			this._id = id;
			this._rev = rev;
			this._deleted = deleted;
		}
		// public abstract void changeElements(PouchData tempItem);
	}
}
