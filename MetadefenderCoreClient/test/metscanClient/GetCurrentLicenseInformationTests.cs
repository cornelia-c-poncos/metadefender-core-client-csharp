
using NUnit.Framework;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Opswat.Metadefender.Core.Client.Responses;
using System.Net;

namespace MetscanClient
{
	[TestFixture]
	public class GetCurrentLicenseInformationTests : BaseHttpMockTest
	{
		[Test]
		public void Success()
		{
			CreateStubForLogin();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			CreateStub("/admin/license", "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.getCurrentLicenseInformation.getCurrentLicenseInformation_success.json"));

			License result = metadefenderCoreClient.GetCurrentLicenseInformation();
			Assert.AreEqual(3740, result.days_left);
			Assert.AreEqual("OPSWAT, Inc.", result.licensed_to);
			Assert.AreEqual(10, result.max_agent_count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/admin/license");
				}
			);
		}

		[Test]
		public void Success_withNewUnknownFields()
		{
			CreateStubForLogin();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			CreateStub("/admin/license", "GET", 200, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.getCurrentLicenseInformation.getCurrentLicenseInformation_withNewUnknownFields.json"));

			License result = metadefenderCoreClient.GetCurrentLicenseInformation();
			Assert.AreEqual(3740, result.days_left);
			Assert.AreEqual("OPSWAT, Inc.", result.licensed_to);
			Assert.AreEqual(10, result.max_agent_count);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/admin/license");
				}
			);
		}

		[Test]
		public void ServerError()
		{
			CreateStubForLogin();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			CreateStub("/admin/license", "GET", 500, GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.errorJson.json"));

			bool isException = false;
			try
			{
				metadefenderCoreClient.GetCurrentLicenseInformation();
			}
			catch (MetadefenderClientException)
			{
				isException = true;
			}
			Assert.True(isException);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/admin/license");
				}
			);
		}

		[Test]
		public void ApiRedirectTest()
		{
			CreateStubForLogin();

			MetadefenderCoreClient metadefenderCoreClient = new MetadefenderCoreClient(GetMockApiUrl(), "admin", "admin");

			HttpServer.Stub(x => x.CustomVerb("/admin/license", "GET"))
				.AddHeader("Content-Type", "application/json; charset=utf-8")
				.AddHeader("Location", "/admin/licenseRedirected")
				.WithStatus((HttpStatusCode) 302);

			// the redirected resource
			HttpServer.Stub(x => x.CustomVerb("/admin/licenseRedirected", "GET"))
				.Return(GetJsonFromFile("MetadefenderCoreClient.test.resources.apiResponses.getCurrentLicenseInformation.getCurrentLicenseInformation_success.json"))
				.AddHeader("Content-Type", "application/json; charset=utf-8")
				.AddHeader("Location", "/admin/licenseRedirected")
				.WithStatus((HttpStatusCode) 200);

			License result = metadefenderCoreClient.GetCurrentLicenseInformation();
			Assert.AreEqual(3740, result.days_left);

			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/admin/license");
				}
			);
			HttpServer.AssertWasCalled(x =>
				{
					return x.CustomVerb("GET", "/admin/licenseRedirected");
				}
			);
		}
	}
}