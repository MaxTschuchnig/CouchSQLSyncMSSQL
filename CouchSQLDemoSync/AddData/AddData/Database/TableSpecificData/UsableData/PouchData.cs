using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddData.Database.TableSpecificData.UsableData
{
	public class PouchData
	{
		public string	_id		{ get; set; }
		[Key]
		public string	_rev	{ get; set; }

		public PouchData() { }
		public PouchData(string id, string rev)
		{
			this._id = id;
			this._rev = rev;
		}


		// public abstract void changeElements(PouchData tempItem);
	}
}
