using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using RestSharp;
using ServiceStack;

namespace ConsumingRestfulApiExamples
{
	public class Program
	{
		private const string SampleUrl = "https://api.github.com/repos/restsharp/restsharp/releases";
		private const string UserAgent = "User-Agent";

		private const string UserAgentValue =
			"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

		public static void Main(string[] args)
		{
			//to support github's depreciation of older cryptographic standards 
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			Console.WriteLine("Select implementation from the following list:");
			Console.WriteLine("1 - HttpWebRequest/Response");
			Console.WriteLine("2 - WebClient");
			Console.WriteLine("3 - HttpClient");
			Console.WriteLine("4 - RestSharp NuGet Package");
			Console.WriteLine("5 - RestSharp NuGet Package (with in-built parser)");
			Console.WriteLine("6 - ServiceStack Http Utils");
			Console.WriteLine("7 - Flurl");
			var userSelection = Console.ReadLine();

			if (userSelection == "1")
			{
				HttpWebRequestHandler();
			}
			else if (userSelection == "2")
			{
				WebClientHandler();
			}
			else if (userSelection == "3")
			{
				HttpClientHandler();
			}
			else if (userSelection == "4")
			{
				RestSharpHandler();
			}
			else if (userSelection == "5")
			{
				RestSharpWithParserHandler();
			}
			else if (userSelection == "6")
			{
				ServiceStackHandler();
			}
			else if (userSelection == "7")
			{
				FlurlHandler();
			}
			else
			{
				Console.WriteLine("Invalid implementation selection");
			}

			Console.ReadLine();
		}

		private static void HttpWebRequestHandler()
		{
			Console.WriteLine("HttpWebRequest/Response selected");

			var request = (HttpWebRequest) WebRequest.Create(SampleUrl);
			request.Method = "GET";
			request.UserAgent = UserAgentValue;
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

			var content = string.Empty;

			using (var response = (HttpWebResponse) request.GetResponse())
			{
				using (var stream = response.GetResponseStream())
				{
					using (var sr = new StreamReader(stream))
					{
						content = sr.ReadToEnd();
						Console.Write(content);
					}
				}
			}
		}

		private static void WebClientHandler()
		{
			Console.WriteLine("WebClient selected");

			var client = new WebClient();
			client.Headers.Add(UserAgent, UserAgentValue);

			var response = client.DownloadString(SampleUrl);

			Console.Write(response);
		}

		private static void HttpClientHandler()
		{
			// intention is to call this method asynchronously - httpClient.GetStringAsync is awaitable
			Console.WriteLine("HttpClient selected");

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add(UserAgent, UserAgentValue);

				var response = httpClient.GetStringAsync(new Uri(SampleUrl)).Result;

				Console.Write(response);
			}
		}

		private static void RestSharpHandler()
		{
			Console.WriteLine("RestSharp NuGet Package selected");

			var client = new RestClient(SampleUrl);

			var response = client.Execute(new RestRequest());

			Console.Write(response.Content);
		}

		private static void RestSharpWithParserHandler()
		{
			Console.WriteLine("RestSharp NuGet Package (with in-built parser) selected");

			var client = new RestClient(SampleUrl);

			var response = client.Execute<List<GitHubRelease>>(new RestRequest());

			Console.Write(response.Content);

			List<GitHubRelease> gitHubReleases = response.Data;
			foreach (var release in gitHubReleases)
			{
				Console.WriteLine("Release: {0}", release.Name);
				Console.WriteLine("Published: {0}", DateTime.Parse(release.PublishedAt));
				Console.WriteLine();
			}
		}

		private static void ServiceStackHandler()
		{
			Console.WriteLine("ServiceStack Http Utils selected");

			var response = SampleUrl.GetJsonFromUrl(request =>
			{
				request.UserAgent = UserAgentValue;
			});

			Console.Write(response);
		}

		private static void FlurlHandler()
		{
			// intention is to call this method asynchronously - GetJsonAsync is awaitable
			Console.WriteLine("Flurl selected");

			var result = SampleUrl
				.WithHeader(UserAgent, UserAgentValue)
				.GetJsonAsync<List<GitHubRelease>>()
				.Result;

			foreach (var release in result)
			{
				Console.WriteLine("Release: {0}", release.Name);
				Console.WriteLine("Published: {0}", DateTime.Parse(release.PublishedAt));
				Console.WriteLine();
			}
		}
	}
}
