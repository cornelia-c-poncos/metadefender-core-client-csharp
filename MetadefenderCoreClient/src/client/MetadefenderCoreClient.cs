

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Opswat.Metadefender.Core.Client;
using Opswat.Metadefender.Core.Client.Exceptions;
using Newtonsoft.Json;

/**
 * Main class for the REST API client
 */

namespace Opswat.Metadefender.Core.Client
{
	public class MetadefenderCoreClient
	{
		private HttpConnector httpConnector = new HttpConnector();

		private string apiEndPointUrl;

		private string sessionId = null;

		// default user_agent for fileScan-s
		private string user_agent = null;

		/**
		 * Constructor for the client.
		 * If you use this constructor, you wont be logged in. (You can use several api calls without authentication)
		 * @param apiEndPointUrl Format: protocol://host:port  Example value: http://localhost:8008
		 */
		public MetadefenderCoreClient(string apiEndPointUrl)
		{
			this.apiEndPointUrl = apiEndPointUrl;
		}

		/**
		 * Constructs a rest client with an api key, to access protected resources.
		 *
		 * @param apiEndPointUrl Format: protocol://host:port  Example value: http://localhost:8008
		 * @param apiKey valid api key
		 */
		public MetadefenderCoreClient(string apiEndPointUrl, string apiKey)
		{
			this.apiEndPointUrl = apiEndPointUrl;

			sessionId = apiKey;
		}


		/**
		 * Constructs a rest client with an api authentication.
		 *
		 * @param apiEndPointUrl Format: protocol://host:port  Example value: http://localhost:8008
		 * @param userName username to login with
		 * @param password password to login with
		 * @throws MetadefenderClientException if the provided user/pass is not valid.
		 */
		public MetadefenderCoreClient(string apiEndPointUrl, string userName, string password)
		{
			this.apiEndPointUrl = apiEndPointUrl;

			Login(userName, password);
		}

		/**
		 * You can set your custom HttpConnector instance to use for making all the requests to the API endpoint.
		 * This can be handy if You want to handle special connection issues yourself.
		 *
		 * @param httpConnector your custom HttpConnector
		 */
		public void SetHttpConnector(HttpConnector httpConnector)
		{
			if (httpConnector == null)
			{
				throw new ArgumentNullException("httpConnector cannot be null");
			}

			this.httpConnector = httpConnector;
		}

		/**
		 * For setting a default user agent string for all file scan api calls.
		 * @param user_agent custom user agent string
		 */
		public void SetUserAgent(string user_agent)
		{
			this.user_agent = user_agent;
		}

		/**
		 * If you successfully log in with username/password you will get a session id.
		 *
		 * @return The current session id.
		 */
		public string GetSessionId()
		{
			return sessionId;
		}


		/**
		 * Initiate a new session for using protected REST APIs.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_login.html" target="_blank">REST API doc</a>
		 *
		 * @param userName username to login with
		 * @param password password to login with
		 * @throws MetadefenderClientException
		 */
		public void Login(string userName, string password)
		{
			Requests.Login loginRequest = new Requests.Login();
			loginRequest.user = userName;
			loginRequest.password = password;

			string requestStr = JsonConvert.SerializeObject(loginRequest);
			MemoryStream requestStream = new MemoryStream(Encoding.UTF8.GetBytes(requestStr));
			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/login", "POST", requestStream, null);
			
			if (response.responseCode == 200)
			{
				try
				{
					Responses.Login loginResponse = JsonConvert.DeserializeObject<Responses.Login>(response.response);
					if (loginResponse != null)
					{
						sessionId = loginResponse.session_id;
					}
					else
					{
						ThrowRequestError(response);
					}
				}
				catch (JsonSerializationException)
				{
					ThrowRequestError(response);
				}
			}
			else
			{
				ThrowRequestError(response);
			}
		}

		/**
		 * Get the current session state.
		 *
		 * @return TRUE if the current session is valid, FALSE otherwise.
		 * @throws MetadefenderClientException
		 */
		public bool ValidateCurrentSession()
		{
			if (string.IsNullOrEmpty(sessionId))
			{
				return false;
			}

			HttpConnector.HttpResponse response =
				httpConnector.SendRequest(apiEndPointUrl + "/version", "GET", null, GetLoggedInHeader());

			return response.responseCode == 200;
		}

		/**
		 * Scan is done asynchronously and each scan request is tracked by data id of which result can be retrieved by API Fetch Scan Result.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_scan_file.html" target="_blank">REST API doc</a>
		 *
		 * @param inputStream Stream of data (file) to scan. Required
		 * @param fileScanOptions Optional file scan options. Can be NULL.
		 * @return unique data id for this file scan
		 */
		public string ScanFile(Stream inputStream, FileScanOptions fileScanOptions)
		{
			if (inputStream == null)
			{
				throw new MetadefenderClientException("Stream cannot be null");
			}

			if ((fileScanOptions != null && fileScanOptions.GetUserAgent() == null) && user_agent != null)
			{
				fileScanOptions.SetUserAgent(user_agent);
			}

			if (fileScanOptions == null && user_agent != null)
			{
				fileScanOptions = new FileScanOptions().SetUserAgent(user_agent);
			}

			Dictionary<string, string> headers = (fileScanOptions == null) ? null : fileScanOptions.GetOptions();

			HttpConnector.HttpResponse response =
				httpConnector.SendRequest(apiEndPointUrl + "/file", "POST", inputStream, headers);

			if (response.responseCode == 200)
			{
				try
				{
					Responses.ScanRequest scanRequest = JsonConvert.DeserializeObject<Responses.ScanRequest>(response.response);
					if (scanRequest != null)
					{
						return scanRequest.data_id;
					}
					else
					{
						ThrowRequestError(response);
						return null;
					}
				}
				catch (JsonSerializationException)
				{
					ThrowRequestError(response);
					return null;
				}
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}

		/**
		 * Scan file in synchron mode.
		 * Note: this method call will block your thread until the file scan finishes.
		 *
		 * @param inputStream input stream to scan
		 * @param fileScanOptions optional file scan options
		 * @param pollingInterval polling time in millis
		 * @param timeout timeout in millis
		 * @return FileScanResult
		 * @throws MetadefenderClientException
		 */
		public Responses.FileScanResult ScanFileSync(Stream inputStream, FileScanOptions fileScanOptions, int pollingInterval,
			int timeout)
		{
			string data_id = ScanFile(inputStream, fileScanOptions);
			var t = Task<Responses.FileScanResult>.Run(() =>
				{
					Responses.FileScanResult fileScanResult;
					do
					{
						fileScanResult = FetchScanResult(data_id);

						if (!fileScanResult.IsScanFinished())
						{
							Thread.Sleep(pollingInterval);
						}
					}
					while (!fileScanResult.IsScanFinished());
					return fileScanResult;
				}
			);

			Task.WhenAny(t, Task.Delay(timeout));
			if (t.Wait(timeout))
			{
				return t.Result;
			}
			else
			{
				throw new TimeoutException();
			}
		}


		/**
		 * Retrieve scan results.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_fetch_scan_result.html" target="_blank"v>REST API doc</a>
		 *
		 * @param data_id Unique file scan id. Required.
		 * @return File scan result object
		 * @throws MetadefenderClientException
		 */
		public Responses.FileScanResult FetchScanResult(string data_id)
		{
			if (string.IsNullOrEmpty(data_id))
			{
				throw new MetadefenderClientException("data_id is required");
			}

			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/file/" + data_id, "GET");

			if (response.responseCode == 200)
			{
				return GetObjectFromJson<Responses.FileScanResult>(response.response);
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}

		/**
		 * Fetch Scan Result by File Hash
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_fetch_scan_result_by_file_hash.html" target="_blank">REST API doc</a>
		 *
		 * @param hash {md5|sha1|sha256 hash}
		 * @return File scan result object
		 * @throws MetadefenderClientException
		 */
		public Responses.FileScanResult FetchScanResultByHash(string hash)
		{
			if (string.IsNullOrEmpty(hash))
			{
				throw new MetadefenderClientException("Hash is required");
			}

			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/hash/" + hash, "GET");

			if (response.responseCode == 200)
			{
				Responses.FileScanResult fileScanResult = GetObjectFromJson<Responses.FileScanResult>(response.response);

				if (fileScanResult == null)
				{
					ThrowRequestError(response);
				}
				return fileScanResult;
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}

		/**
		 * You need to be logged in to access this API point.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_getlicense.html" target="_blank">REST API doc</a>
		 *
		 * @return License object
		 * @throws MetadefenderClientException
		 */
		public Responses.License GetCurrentLicenseInformation()
		{
			CheckSession();

			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/admin/license", "GET", null,
				GetLoggedInHeader());

			if (response.responseCode == 200)
			{
				return GetObjectFromJson<Responses.License>(response.response);
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}

		/**
		 * Fetching Engine/Database Versions
		 * The response is an array of engines with database information.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_fetching_engine_database_versions.html" target="_blank">REST API doc</a>
		 *
		 * @return List of Engine versions
		 * @throws MetadefenderClientException
		 */
		public List<Responses.EngineVersion> GetEngineVersions()
		{
			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/stat/engines", "GET");

			if (response.responseCode == 200)
			{
				return GetObjectFromJson<List<Responses.EngineVersion>>(response.response);
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}


		/**
		 * You need to be logged in to access this API point.
		 *
		 * @return ApiVersion object
		 * @throws MetadefenderClientException
		 */
		public Responses.ApiVersion GetVersion()
		{
			CheckSession();

			HttpConnector.HttpResponse response =
				httpConnector.SendRequest(apiEndPointUrl + "/version", "GET", null, GetLoggedInHeader());

			if (response.responseCode == 200)
			{
				return GetObjectFromJson<Responses.ApiVersion>(response.response);
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}

		/**
		 * Fetching Available Scan Rules
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_fetching_available_scan_workflows.html" target="_blank">REST API doc</a>
		 *
		 * @return List of Scan rules
		 * @throws MetadefenderClientException
		 */
		public List<Responses.ScanRule> GetAvailableScanRules()
		{
			HttpConnector.HttpResponse response = httpConnector.SendRequest(apiEndPointUrl + "/file/rules", "GET");

			if (response.responseCode == 200)
			{
				return GetObjectFromJson<List<Responses.ScanRule>>(response.response);
			}
			else
			{
				ThrowRequestError(response);
				return null;
			}
		}


		/**
		 * You need to be logged in to access this API point.
		 *
		 * Destroy session for not using protected REST APIs.
		 *
		 * @see <a href="http://software.opswat.com/metascan/Documentation/Metascan_v4/documentation/user_guide_metascan_developer_guide_logout.html" target="_blank">REST API doc</a>
		 * @throws MetadefenderClientException
		 */
		public void Logout()
		{
			CheckSession();

			HttpConnector.HttpResponse response =
				httpConnector.SendRequest(apiEndPointUrl + "/logout", "POST", null, GetLoggedInHeader());
			sessionId = null;

			if (response.responseCode != 200)
			{
				ThrowRequestError(response);
			}
		}

		//////// Private utils methods:
		private Dictionary<string, string> GetLoggedInHeader()
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers["apikey"] = sessionId;
			return headers;
		}

		/**
		 * Generic error handling for API request where response code != 200
		 * @param response actual API response
		 * @throws MetadefenderClientException
		 */
		private void ThrowRequestError(HttpConnector.HttpResponse response)
		{
			try
			{
				Responses.Error error = JsonConvert.DeserializeObject<Responses.Error>(response.response);
				if (error != null)
				{
					throw new MetadefenderClientException(error.err, response.responseCode);
				}
				else
				{
					throw new MetadefenderClientException("", response.responseCode);
				}
			}
			catch (JsonSerializationException)
			{
				throw new MetadefenderClientException("", response.responseCode);
			}
		}

		/**
		 * Reusable util method for current session exist check
		 * @throws MetadefenderClientException if session is empty
		 */
		private void CheckSession()
		{
			if (string.IsNullOrEmpty(sessionId))
			{
				throw new MetadefenderClientException("You need to be logged in to access this API point.");
			}
		}

		private T GetObjectFromJson<T>(string json) where T : class
		{
			try
			{
				T ret = JsonConvert.DeserializeObject<T>(json);
				if (ret != null)
				{
					return ret;
				}
				else
				{
					throw new MetadefenderClientException("Error serializing Json: " + json);
				}
			}
			catch (JsonSerializationException e)
			{
				throw new MetadefenderClientException("Error: " + e.Message + " Json: " + json);
			}
		}
	}
}
