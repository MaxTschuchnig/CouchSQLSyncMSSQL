using CouchSQLSync_v4.Database;
using CouchSQLSync_v4.Database.TableSpecificData.Changes;
using CouchSQLSync_v4.Database.TableSpecificData.ReplicationLog;
using CouchSQLSync_v4.Database.TableSpecificData.UsableData;
using CouchSQLSync_v4.Database.UniversalData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CouchSQLSync_v4.SyncMiddleware
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	[RoutePrefix("api")]
	[AllowAnonymous]
	public class MessageController : ApiController
	{
		#region already done
		[HttpGet]
		[Route("")]
		public HttpResponseMessage GetPeersInformation()
		{
			try
			{
				using (MessageControllerHelper helper = new MessageControllerHelper())
				{
					ConnectorInfo TempConnI = new ConnectorInfo();
					try
					{
						// TODO: Secure this, what if db info gets deleted while programm is running
						using (DatabaseContext db = new DatabaseContext())
							TempConnI = db.Info.First();
					}
					catch (Exception e)
					{
						Console.WriteLine("Error when getting data from Info, e: " + e.Message);
						throw new Exception(e.Message);
					}

					object tempRetObject = new
					{
						instance_start_time = TempConnI.InstanceStartTime,
						update_seq = TempConnI.UpdateSeq,
						version = TempConnI.Version,
						vendor = TempConnI.Vendor
					};
					return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(tempRetObject)));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception in Init Db, e: " + e.Message);

				object tempRetObject = new { exception = e.Message };
				using (MessageControllerHelper helper = new MessageControllerHelper())
					return helper.GetJsonResponse(HttpStatusCode.InternalServerError, new StringContent(JsonConvert.SerializeObject(tempRetObject)));
			}
		}

		[HttpGet]
		[Route("{database}/_local/{documentId}")]
		public HttpResponseMessage RetriveReplicationLogs(string database, string documentId)
		{
			try
			{
				bool found = false;
				List<ReplicationLog> ReplicationLogs;

				try
				{
					using (DatabaseContext db = new DatabaseContext())
					{
						ReplicationLogs = db.ReplicationLog.Where(entity => entity.DatabaseName.Equals(database) && entity.DocumentId.Equals(documentId)).ToList();
						if (ReplicationLogs.Count > 0)
							found = true;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception in RetriveReplicationLogs, e: " + e.Message);
					throw new Exception(e.Message);
				}

				object temp;
				if (found)
				{
					try
					{
						// TODO: Implement Parse of the whole replication log, for more info look in v3, already done there but not complete
						using (ReplicationLogParser RepParser = new ReplicationLogParser())
							temp = RepParser.Parse(ReplicationLogs);

						using (MessageControllerHelper helper = new MessageControllerHelper())
							return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(temp)));
					}
					catch (Exception e)
					{
						Console.WriteLine("Exception in RetriveReplicationLogs, e: " + e.Message);
						throw new Exception(e.Message);
					}
				}
				else
				{
					temp = new
					{
						error = "not_found",
						reason = "missing"
					};

					using (MessageControllerHelper helper = new MessageControllerHelper())
						return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(temp)));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception in RetriveReplicationLogs" + e.Message);

				object tempRetObject = new { exception = e.Message };
				using (MessageControllerHelper helper = new MessageControllerHelper())
					return helper.GetJsonResponse(HttpStatusCode.InternalServerError, new StringContent(JsonConvert.SerializeObject(tempRetObject)));
			}
		}

		// TODO: Only return latest changes
		[HttpGet]
		[Route("{db}/_changes")]
		public HttpResponseMessage GetChanges(string db, string style, string since, int limit)
		{
			Console.WriteLine("In Changes, db: " + db + ", style: " + style + ", since: " + since + ", limit: " + limit);

			// Get Revisions from DB
			List<Change> revsList = new List<Change>();

			// Get Data for revision Ids
			List<PouchData> pouchDataList = new List<PouchData>();
			using (DatabaseContext _db = new DatabaseContext())
			{
				// only take limit if there is more than limit in db
				int RevsCount = _db.Changes.Where(e => e.DatabaseName.Equals(db)).ToList().Count();
				if (RevsCount < limit)
					limit = RevsCount;

				revsList = _db.Changes.Where(e => e.DatabaseName.Equals(db)).OrderByDescending(e => e.seq).Take(limit).ToList();

				// Remove all before Since
				using (MessageControllerHelper helper = new MessageControllerHelper())
				{
					if (!since.Equals("") && !since.Equals("0"))
						revsList = helper.FindRevsAfterSince(since, revsList);
				}


				// Get Data to look for revision ids
				// TODO: Smarter solution!!
				if (db.Equals("Tickets"))
				{
					// not sure if limit here would be working as intended as i don't know the ordering
					// _db.Tickets.Take(limit).ToList();

					// take the list and convert it into a json string
					var tempString = JsonConvert.SerializeObject(_db.Tickets.ToList());

					// If we now deserialize it, only the inclusive Parameters stay
					pouchDataList = JsonConvert.DeserializeObject<List<PouchData>>(tempString);
				}
				else
				{
					Console.WriteLine("ERROR!!! Read TODOs. We may have other stuff than only Tickets");
				}
			}

			// Get highest Sequence Number (Find latest Change)
			int _seq = 0, highestSequenceId = 0;
			string highestSequence = "";
			List<Change> tempRevsList = new List<Change>();
			foreach (Change cChange in revsList)
			{
				_seq = 0;
				using (MessageControllerHelper helper = new MessageControllerHelper())
				{
					_seq = helper.ParseSeqId(cChange.seq);
				}
				if (highestSequenceId < _seq)
				{
					highestSequenceId = _seq;
					highestSequence = cChange.seq;
				}

				// Remove first element since it is the same
				if (!since.Equals("") && !since.Equals("0"))
					continue;

				tempRevsList.Add(cChange);
			}
			revsList = tempRevsList;

			using (MessageControllerHelper helper = new MessageControllerHelper())
			{
				string temp;
				using (ReturnParser parser = new ReturnParser())
				{
					temp = parser.GetChangesReturn(revsList, pouchDataList, highestSequence);
				}
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(temp));
			}
		}

		[HttpPost]
		[Route("{db}/_revs_diff")]
		public HttpResponseMessage GetRevisionDifferences(string db, [FromBody]object msg)
		{
			// Parse Post Message in correct format (dict of ids with rev arrays)
			JObject parameters = JObject.Parse(msg.ToString());
			Dictionary<string, object> parametersDict = parameters.ToObject<Dictionary<string, object>>();

			// Get used Ids
			List<string> Ids = new List<string>();
			foreach (string cId in parametersDict.Keys)
				Ids.Add(cId);

			// Find revs for every id
			List<List<string>> Revisions = new List<List<string>>();
			foreach (string cId in Ids)
				Revisions.Add(JsonConvert.DeserializeObject<List<string>>(parametersDict[cId].ToString()));

			// Find missing revisions and Ids
			List<Tuple<string, List<string>>> Missing = new List<Tuple<string, List<string>>>();
			using (DatabaseContext _db = new DatabaseContext())
			{
				List<string> cMissingRevs = new List<string>();

				// Check each id if it exists
				for (int i = 0; i < Ids.Count; i++)
				{
					List<Ticket> ctempData = _db.Tickets.ToList();

					// Find missing ids in general
					List<string> allIds = new List<string>();
					foreach (Ticket cTicket in ctempData)
						allIds.Add(cTicket._id);

					// Is completely missing add all and break
					bool foundId = false;
					foreach (string id in Ids)
					{
						if (id.Equals(Ids[i]))
						{
							if (!allIds.Contains(id))
							{
								Missing.Add(new Tuple<string, List<string>>(id, Revisions[i]));
								foundId = true;
								continue;
							}
						}
					}
					if (foundId)
						continue;

					
					// bool missingFound = false;

					// Get revision List
					List<string> allRevisions = new List<string>();
					foreach (Ticket cData in ctempData)
						allRevisions.Add(cData._rev);

					// Check all for missing ones
					cMissingRevs = new List<string>();
					foreach (string cRevision in Revisions[i])
					{
						if (!allRevisions.Contains(cRevision))
							cMissingRevs.Add(cRevision);
					}

					Missing.Add(new Tuple<string, List<string>>(Ids[i], cMissingRevs));

					/*
					foreach (Ticket cData in ctempData)
						if (cData._id.Equals(Ids[i]))
						{
							foreach (string cRevision in Revisions[i])
							{
								if (!cRevision.Equals(cData._rev))
								{
									Console.WriteLine("Found Missing Rev: " + cRevision);

									cMissingRevs.Add(cRevision);
									missingFound = true;
								}
							}
						}

					if (missingFound)
						Missing.Add(new Tuple<string, List<string>>(Ids[i], cMissingRevs));
					*/
				}
			}

			using (MessageControllerHelper helper = new MessageControllerHelper())
			{
				string temp;
				using (ReturnParser parser = new ReturnParser())
				{
					temp = parser.GetRevisionDifferenceReturns(Missing);
				}
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(temp));
			}
		}
		
		[HttpPost]
		[Route("{dbName}/_bulk_docs")]
		public HttpResponseMessage BulkAllDocs(string dbName, [FromBody]object msg)
		{
			Console.WriteLine("Adding Bulk of new Docs");

			JObject msgJson = JObject.Parse(msg.ToString());

			var data = msgJson["docs"];
			List<PouchData> newData = new List<PouchData>();
			foreach (var cPouchData in data)
			{
				Console.WriteLine(JsonConvert.SerializeObject(cPouchData));

				//TODO: Make this smarter!!!
				if (dbName.Equals("Tickets"))
					newData.Add(JsonConvert.DeserializeObject<Ticket>(JsonConvert.SerializeObject(cPouchData)));
			}

			using (DatabaseContext db = new DatabaseContext())
			{
				if (dbName.Equals("Tickets"))
				{
					List<Ticket> ticketsToAdd = new List<Ticket>();
					foreach (Ticket cTicket in newData)
					{
						if (MessageController.AlreadyExists(cTicket, "Tickets"))
						{
							// tempItemToAdd = DatabaseHelper.Instance.EditItem(tempItem, "Tickets");
							Console.WriteLine(cTicket._rev + " already exists, skipping (change to edit if different)");
							continue;
						}
						ticketsToAdd.Add(cTicket);
					}
					db.Tickets.AddRange(ticketsToAdd);
				}
				db.SaveChanges();
			}

			// TODO: Add new edits. What is it used for?
			// msgJson["new_edits"];

			// Add new Changes
			foreach (PouchData cPouchData in newData)
			{
				using (MessageControllerHelper helper = new MessageControllerHelper())
				{
					helper.AddNewChange(cPouchData, dbName);
				}
			}

			using (MessageControllerHelper helper = new MessageControllerHelper())
			{
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent("[]"));
			}
		}

		[HttpPost]
		[Route("{dbName}/_all_docs")]
		public HttpResponseMessage GetAllDocs(string dbName, bool conflicts, bool include_docs, [FromBody]object msg)
		{
			// TODO: all docs returns too much!!!
			// Maybe unambigous because mulitple docs with same id returned!!!
			// TODO: FIX!

			Console.WriteLine(msg.ToString());
			JObject AskedForIds = JObject.Parse(msg.ToString());
			var AskedForIdsParsed = AskedForIds["keys"];
			List<string> AskedIdsList = AskedForIdsParsed.ToObject<List<string>>();

			List<List<PouchData>> AskedDataList = new List<List<PouchData>>();
			using(DatabaseContext db = new DatabaseContext())
			{
				foreach (string cId in AskedIdsList)
				{
					List<PouchData> cDataList = new List<PouchData>();

					// TODO: make smarter
					if (dbName.Equals("Tickets"))
					{
						cDataList.AddRange(db.Tickets.Where(entity => entity._id.Equals(cId)).ToList());
					}
					AskedDataList.Add(cDataList);
				}
			}

			string retMessage = "";
			using (ReturnParser parser = new ReturnParser())
			{
				retMessage = parser.ParseAllDocsReturn(AskedDataList);
			}

			using (MessageControllerHelper helper = new MessageControllerHelper())
			{
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(retMessage));
			}
		}
		
		[HttpPost]
		[Route("{dbName}/_bulk_get")]
		public HttpResponseMessage BulkGetDocs(string dbName, bool revs, bool latest, [FromBody]object msg)
		{
			Console.WriteLine("Bulk Get");

			Console.WriteLine(msg.ToString());
			JObject msgJson = JObject.Parse(msg.ToString());
			var data = msgJson["docs"];
			// TODO: CHANGE TO ALSO GET MuLTIPLE AT ONCE!!!
			int length = data.Count();
			List<string> _ids = new List<string>();
			List<string> _revs = new List<string>();


			List<object> results = new List<object>();


			for (int i = 0; i < length; i ++)
			{
				_ids.Add(data[i]["id"].ToString());
				_revs.Add(data[i]["rev"].ToString());

				// Returns id and latest doc with given id and rev
				List<PouchData> cData = new List<PouchData>();
				using (DatabaseContext db = new DatabaseContext())
				{
					if (dbName.Equals("Tickets"))
					{
						List<Ticket> ctempData = db.Tickets.ToList();

						foreach (Ticket cTicket in ctempData)
						{
							if (cTicket._rev.Equals(_revs[i]) && cTicket._id.Equals(_ids[i]))
								cData.Add(cTicket);
						}
					}
				}

				List<object> cDocs = new List<object>();
				using (ReturnParser parser = new ReturnParser())
				{
					cDocs = parser.ParseGetDocs(cData, _ids[i]);
				}

				object cObject = new
				{
					id = _ids[i],
					docs = cDocs
				};
				results.Add(cObject);
			}

			using (MessageControllerHelper helper = new MessageControllerHelper())
			{
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(new { results = results })));
			}
		}
		#endregion

			#region out_of_focus
		[HttpGet]
		[Route("{db}")]
		public HttpResponseMessage CheckExistence(string db)
		{
			// TODO: check correclty. Check in datacontext.ExistingTables.Contains
			bool exists = false;
			// TODO: check correctly
			bool insufficientPriviledges = false;

			object temp;

			if (insufficientPriviledges)
			{
				temp = new { error = "unauthorized", reason = "could not open unauthorized to access or create database" };
				using (MessageControllerHelper helper = new MessageControllerHelper())
					return helper.GetJsonResponse(HttpStatusCode.Unauthorized, new StringContent(JsonConvert.SerializeObject(temp)));
			}

			if (exists)
			{
				temp = new { ok = true };
				using (MessageControllerHelper helper = new MessageControllerHelper())
					return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(temp)));
			}
			else
			{
				temp = new { error = "db_not_found", reason = "could not open source" };
				using (MessageControllerHelper helper = new MessageControllerHelper())
					return helper.GetJsonResponse(HttpStatusCode.NotFound, new StringContent(JsonConvert.SerializeObject(temp)));
			}
		}

		[HttpPut]
		[Route("{db}")]
		public HttpResponseMessage PutDatabase(string db)
		{
			// TODO: Check if DB already exists
				// If it does, do nothing
			// TODO: Generate Table
				// As we are using EF, the database already has to exist before

			object temp = new { ok = "ok" };
			using (MessageControllerHelper helper = new MessageControllerHelper())
				return helper.GetJsonResponse(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(temp)));
		}
		#endregion

		// TODO: move somewhere else
		internal static bool AlreadyExists(PouchData tempItem, string dbName)
		{
			using (DatabaseContext db = new DatabaseContext())
			{
				// TODO: make smarter
				if (dbName.Equals("Tickets"))
				{
					var result = db.Tickets.SingleOrDefault(t => t._rev.Equals(tempItem._rev));
					if (result == null)
						return false;
				}
				return true;
			}
		}
	}
}