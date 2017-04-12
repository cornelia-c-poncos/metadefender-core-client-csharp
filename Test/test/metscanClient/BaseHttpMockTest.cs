
using System.IO;
using NUnit.Framework;
using System.Reflection;
using System.Text;
using HttpMock;
using HttpMock.Verify.NUnit;
using System.Net;

namespace MetscanClient
{
	[TestFixture]
	public class BaseHttpMockTest
	{
		public const string TEST_SESSION_ID = "22c4b45a38a449628247c9d431b48fcd";

		public const int Port = 60008;

		IHttpServer httpServer;
		protected IHttpServer HttpServer
		{
			get
			{
				return httpServer;
			}
		}

		[SetUp]
		public void InitHttpMock()
		{
			if (httpServer == null)
			{
				httpServer = HttpMockRepository.At("http://localhost:" + Port);
			}
			else
			{
				httpServer.WithNewContext();
			}
		}

		[TearDown]
		public void DisposeHttpMock()
		{
		}

		protected string GetMockApiUrl()
		{
			return "http://localhost:" + Port;
		}

		protected void CreateStubForLogin()
		{
			CreateStub("/login", "POST", 200, "{\"session_id\":\"" + TEST_SESSION_ID + "\"}");
		}

		protected void CreateStubForVersion()
		{
			CreateStub("/version", "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.version.getVersion_success.json"));
		}

		protected void CreateStub(string uri, string method, int status, string json)
		{
			httpServer.Stub(x => x.CustomVerb(uri, method))
				.Return(json)
				.AddHeader("Content-Type", "application/json; charset=utf-8")
				.WithStatus((HttpStatusCode) status);
		}

		protected string GetJsonFromFile(string fileName)
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
			{
				MemoryStream memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				return Encoding.ASCII.GetString(memoryStream.ToArray());
			}
		}
	}
}
