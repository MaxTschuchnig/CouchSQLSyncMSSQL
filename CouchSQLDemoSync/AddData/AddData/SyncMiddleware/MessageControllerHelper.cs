using AddData.Database;
using AddData.Database.TableSpecificData.Changes;
using AddData.Database.TableSpecificData.UsableData;
using AddData.Database.UniversalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AddData.SyncMiddleware
{
	class MessageControllerHelper : IDisposable
	{
		public void Init()
		{
			// Init Db, reduces waiting time on first user
			using (DatabaseContext db = new DatabaseContext())
			{
				if (!db.Info.Any())
				{
					VendorInfo TempVendorI = new VendorInfo();
					TempVendorI.name = "FH-Salzburg";
					ConnectorInfo TempConnI = new ConnectorInfo();
					TempConnI.PeerUUID = Guid.NewGuid().ToString();

					TempConnI.InstanceStartTime = 0;
					TempConnI.UpdateSeq = "relpace-with-latest-update-seq";
					TempConnI.Version = "0.1.4";
					TempConnI.Vendor = TempVendorI;

					try
					{
						db.Info.Add(TempConnI);

						// Check again, here we have a mad racing condition as pouchdb always checks twice
						if (!db.Info.Any())
							db.SaveChanges();
					}
					catch (Exception e)
					{
						Console.WriteLine("Error when adding data to Info, e: " + e.Message);
					}
				}
			}
		}

		/// <summary>
		///  We create the Replication Id by adding together the replicator id, the remote uri, the user Ip and the current Timestamp. Then, to obfuscate, we take the sha 256 from this string and store it
		/// </summary>
		/// <param name="UserIp"></param>
		/// <param name="RequestUri"></param>
		/// <param name="Direction"></param>
		/// <returns></returns>
		public string GetReplicationId(string UserIp, string RequestUri, char Direction)
		{
			// Find Client used URI by this.Url.Request.RequestUri.AbsolutePath
			// TODO: Find Client IP (maybe Request.source)
			char Middleware = 'R';

			// They are local, means we are the remote endpoint that they are trying to sync to
			// This could also be the other way around, where we are the ones trying to sync with a remote endoint, the requested Uri
			if (Direction.Equals('L'))
				Middleware = 'R';
			else
				Middleware = 'L';

			string Uuid = "";
			// Get Universal UUID of the Middleware
			using (DatabaseContext db = new DatabaseContext())
			{
				ConnectorInfo tempCInfo = db.Info.FirstOrDefault();
				Uuid = tempCInfo.PeerUUID;
			}

			using(Hashing h = new Hashing())
				return h.GenerateSHA256String(Uuid + Middleware.ToString() + RequestUri + Direction.ToString() + DateTime.UtcNow.ToString() + UserIp);
		}

		/// <summary>
		/// Generates a basic HTTPResonseMessage in json format.
		/// </summary>
		/// <param name="code">Status Code</param>
		/// <param name="content">Content as StringContent</param>
		/// <returns>HttpResponseMessage in json format with status code = code and content = content</returns>
		public HttpResponseMessage GetJsonResponse(HttpStatusCode code, StringContent content)
		{
			var response = new HttpResponseMessage(code);

			if (response.Content == null)
			{
				response.Content = content;
			}

			response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			response.Headers.Add("Cache-Control", "must-revalidate");

			return response;
		}

		/// <summary>
		/// Generates a basic HTTPResonseMessage in html format.
		/// </summary>
		/// <param name="code">Status Code</param>
		/// <param name="content">Content as StringContent</param>
		/// <returns>HttpResponseMessage in html format with status code = code and content = content</returns>
		public HttpResponseMessage GetHTMLResponse(HttpStatusCode code, StringContent content)
		{
			var response = new HttpResponseMessage(code);

			if (response.Content == null)
			{
				response.Content = content;
			}
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

			return response;
		}

		/// <summary>
		/// TODO:
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public List<Tuple<string, string>> getTupelListFromStringLists(List<string> keys, List<string> values)
		{
			List<Tuple<string, string>> ret = new List<Tuple<string, string>>();

			if (keys.Count != values.Count)
				throw new Exception("Different Size, Key - Value");

			for (int i = 0; i < keys.Count; i++)
			{
				ret.Add(new Tuple<string, string>(keys[i], values[i]));
			}

			return ret;
		}

		/// <summary>
		/// TODO:
		/// </summary>
		/// <param name="seqId"></param>
		/// <returns></returns>
		public int ParseSeqId(string seqId)
		{
			int _seq = 0;
			try
			{
				_seq = Int32.Parse(seqId);
			}
			catch
			{
				string[] seqParts = seqId.Split('-');

				try
				{
					_seq = Int32.Parse(seqParts[0]);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			return _seq;
		}

		public string ParseSeq(string seq)
		{
			if (seq.Contains('-'))
			{
				string[] seqParts = seq.Split('-');
				return seqParts[0];
			}
			else
				return seq;
		}

		public int GetHighestSequenceId ()
		{
			List<Change> allChanges = new List<Change>();
			using(DatabaseContext db = new DatabaseContext())
			{
				allChanges = db.Changes.ToList();
			}

			if (allChanges == null)
				return 1;

			int highestSequenceId = 0;
			foreach(Change cChange in allChanges)
			{
				int cSequenceId = this.ParseSeqId(cChange.seq);
				if (cSequenceId > highestSequenceId)
					highestSequenceId = cSequenceId;
			}
			return highestSequenceId +1;
		}

		public void AddNewChange(PouchData CPouchData, string dbName)
		{
			List<PouchData> data = new List<PouchData>();
			Change currentSequence = new Change();
			using (DatabaseContext db = new DatabaseContext())
			{
				currentSequence = db.Changes.Where(entity => entity.id.Equals(CPouchData._id)).SingleOrDefault();

				// TODO: Make smarter!
				if (dbName.Equals("Tickets"))
				{
					List<Ticket> cTickets = db.Tickets.Where(entity => entity._id.Equals(CPouchData._id)).ToList();
					if (cTickets != null)
						foreach (Ticket cTicket in cTickets)
							data.Add(cTicket);
				}
			}

			Change ChangeToAdd = new Change();
			// if here, currentSequence and newestPouchData = null, we create a new Change
			if (currentSequence == null) // && newestPouchData == null)
			{
				ChangeToAdd.id = CPouchData._id;
				ChangeToAdd.DatabaseName = dbName;
				ChangeToAdd.seq = this.GetHighestSequenceId() + "-" + Guid.NewGuid().ToString();
				ChangeToAdd.deleted = false;
				ChangeToAdd.changes = new List<ChangeStringHelper> { new ChangeStringHelper(CPouchData._rev) };
			}
			else
			{
				Console.WriteLine("This has not been added but edited, what do we do here?");
				ChangeToAdd.id = CPouchData._id;
				ChangeToAdd.DatabaseName = dbName;
				ChangeToAdd.seq = this.GetHighestSequenceId() + "-" + currentSequence.seq;
				ChangeToAdd.changes = new List<ChangeStringHelper> { new ChangeStringHelper(CPouchData._rev) };

				// TODO: Change to true if deleted was true!
				ChangeToAdd.deleted = false;
			}

			using(DatabaseContext db = new DatabaseContext())
			{
				db.Changes.Add(ChangeToAdd);
				db.SaveChanges();
			}
		}

		public void Dispose()
		{
		}
	}
}
