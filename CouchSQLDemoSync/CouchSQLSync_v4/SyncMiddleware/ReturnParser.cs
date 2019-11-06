using CouchSQLSync_v4.Database;
using CouchSQLSync_v4.Database.TableSpecificData.Changes;
using CouchSQLSync_v4.Database.TableSpecificData.UsableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouchSQLSync_v4.SyncMiddleware
{
	class ReturnParser : IDisposable
	{
		public string GetChangesReturn(List<Change> changes,List<PouchData> data, string highestSequenceId)
		{
			// return current Row
			List<object> tempRet = new List<object>();
			List<List<string>> allRevsOfIdFiltered = new List<List<string>>();

			foreach (Change cChange in changes)
			{
				string currentSeq = cChange.seq;

				// Get each rev of with same id as current change
				List<string> cRevsOfId = new List<string>();
				List<string> cRevsOfIdFiltered = new List<string>();
				foreach (PouchData cPouchData in data)
					if (cChange.id.Equals(cPouchData._id) && GetSequenceOfPouchItem(cPouchData) == currentSeq)
					{
						cRevsOfId.Add(cPouchData._rev);
						continue;
					}

				// Find highest rev
				int highestRev = 0;
				foreach(string cRev in cRevsOfId)
				{
					using (MessageControllerHelper helper = new MessageControllerHelper())
					{
						int cId = helper.ParseSeqId(cRev);
						if (cId > highestRev)
							highestRev = cId;
					}
				}

				foreach(string cRev in cRevsOfId)
				{
					using (MessageControllerHelper helper = new MessageControllerHelper())
					{
						if (helper.ParseSeqId(cRev) == highestRev)
						{
							cRevsOfIdFiltered.Add(cRev);
						}
					}
				}
				allRevsOfIdFiltered.Add(cRevsOfIdFiltered);
			}

			bool[] doubles = new bool[changes.Count];
			// List<Tuple<string, string>> changes_unique = new List<Tuple<string, string>>();
			for (int i = 0; i < changes.Count; i ++)
			{
				Change cChange = changes[i];
				foreach (Change cInnerChange in changes)
				{
					if (cChange.id.Equals(cInnerChange.id))
					{
						// We found the original
						if (cChange.seq.Equals(cInnerChange.seq))
							continue;

						// We found a double
						using (MessageControllerHelper helper = new MessageControllerHelper())
						{
							if (helper.ParseSeqId(cChange.seq) < helper.ParseSeqId(cInnerChange.seq))
							{
								doubles[i] = true;
							}
						}
					}
				}
			}

			changes.Reverse();
			Array.Reverse(doubles);
			allRevsOfIdFiltered.Reverse();

			for (int i = 0; i < changes.Count; i ++)
			{
				if (doubles[i] == true)
					continue;

				// Build return
				List<object> _changes = new List<object>();
				foreach (string cRevision in allRevsOfIdFiltered[i])
				{
					object revTemp = new
					{
						rev = cRevision
					};
					_changes.Add(revTemp);
				}
				object cRow = new
				{
					seq = changes[i].seq,
					id = changes[i].id,
					changes = _changes
				};
				tempRet.Add(cRow);
			}

			object ret = new
			{
				results = tempRet,
				last_seq = highestSequenceId,
				pending = 0 // 0 for now, don't now why
			};

			return JsonConvert.SerializeObject(ret);
		}

		private string GetSequenceOfPouchItem(PouchData cPouchData)
		{
			List<Change> AllChanges = new List<Change>();
			using (DatabaseContext db = new DatabaseContext())
			{
				AllChanges = db.Changes.ToList();

				// First find corresponding ChangeHelper

				foreach(Change cChange in AllChanges) 
					foreach(ChangeStringHelper cHelper in cChange.changes)
					{
						if (cHelper.revision.Equals(cPouchData._rev))
							return cChange.seq;
					}
			}
			return null;
		}

		public string GetRevisionDifferenceReturns(List<Tuple<string, List<string>>> missing)
		{
			var highestLayer = new ExpandoObject() as IDictionary<string, Object>;
			foreach (Tuple<string, List<string>> cMissing in missing)
			{
				dynamic cMissingInner = new
				{
					missing = cMissing.Item2
				};


				highestLayer.Add(cMissing.Item1, cMissingInner);
			}
			return JsonConvert.SerializeObject(highestLayer);
		}

		public void Dispose()
		{
		}

		internal string ParseAllDocsReturn(List<List<PouchData>> askedDataList)
		{
			// Get all Data

			List<PouchData> retPouchData = new List<PouchData>();
			// Iterate over whole list and only take highest rev ticket
			int highestRev;
			foreach (List<PouchData> cPouchDataList in askedDataList)
			{
				highestRev = 0;
				PouchData pouchDataToAdd = new PouchData();
				foreach (PouchData cPouchData in cPouchDataList)
				{
					using (MessageControllerHelper helper = new MessageControllerHelper())
					{
						if (helper.ParseSeqId(cPouchData._rev) > highestRev)
						{
							highestRev = helper.ParseSeqId(cPouchData._rev);
							pouchDataToAdd = cPouchData;
						}
					}
					retPouchData.Add(pouchDataToAdd);
				}
			}

			// Parse data into correct string
			string retMessage = "";

			// TODO: Find out if this is correct
			// Returns + 1 (First Row is extra, i guess)
			int totalrows = retPouchData.Count + 1;

			List<object> rows = new List<object>();
			foreach(PouchData cPouchData in retPouchData)
			{
				object cData = new
				{
					id = cPouchData._id,
					key = cPouchData._id,
					value = new
					{
						rev = cPouchData._rev
					},
					doc = cPouchData
				};
				rows.Add(cData);
			}

			object ret = new
			{
				total_rows = totalrows,
				rows = rows
			};
			retMessage = JsonConvert.SerializeObject(ret);

			return retMessage;
		}

		internal List<object> ParseGetDocs(List<PouchData> Data, string id)
		{
			List<object> results = new List<object>();
			foreach (PouchData cData in Data)
			{
				// TODO: Revisions in ok missing !!! Add them
				results.Add(new { ok = cData });
			}


			return results;
		}
	}
}
