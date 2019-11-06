using CouchSQLSync_v4.SyncMiddleware;
using CouchSQLSync_v4.SyncMiddleware.WebInterfaceConfig;
using System;
using Topshelf;

namespace CouchSQLSync_v4
{
	class Program
	{
		static void Main(string[] args)
		{
			// Init Db to remove wait time for first user
			using (MessageControllerHelper tempMessageC = new MessageControllerHelper())
				tempMessageC.Init();

			/*
			using (MessageControllerHelper tempMessageC = new MessageControllerHelper())
				Console.WriteLine(tempMessageC.GetReplicationId("192.168.0.2", "/api/Tickets/_local/3jf834f9fj389csduiferidfz34", 'L'));
			*/

			HostFactory.Run(x =>
			{
				x.Service<WebInterfaceService>(s =>
				{
					s.ConstructUsing(name => new WebInterfaceService());
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});
				x.RunAsLocalSystem();

				x.SetDescription("Web Api for JSON communication");
				x.SetDisplayName("Web Api");
				x.SetServiceName("Web Api");
			});

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
