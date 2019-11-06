using Microsoft.Owin.Hosting;
using System;

namespace CouchSQLSync_v4.SyncMiddleware.WebInterfaceConfig
{
	class WebInterfaceService
	{
		private IDisposable _webApp;

		public void Start()
		{
			String url = "http://localhost:12345";

			_webApp = WebApp.Start<OwinStartup>(url);
		}

		public void Stop()
		{
			_webApp.Dispose();
		}
	}
}
