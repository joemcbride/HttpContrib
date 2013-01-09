namespace HttpContrib.Http
{
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Net;
	using System.Runtime.Serialization;
	using System.Threading.Tasks;
	using System.Xml.Serialization;
	
	public static class HttpExtensions
	{
		public static T ReadAsObject<T>(this HttpResponseMessage response)
		{
			if (IsJsonContent(response))
				return DeserializeFromJson<T>(response);

			if (IsXmlContent(response))
				return DeserializeFromXml<T>(response);

			throw new NotSupportedException("The response type is not supported.");
		}

		public static T ReadXmlAsObject<T>(this HttpResponseMessage response)
		{
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(response.GetResponseStream());
		}

		public static Stream WriteObjectAsXml<T>(this T obj)
		{
			var serializer = new XmlSerializer(typeof(T));

			var stream = new MemoryStream();
			serializer.Serialize(stream, obj);
			stream.Position = 0;

			return stream;
		}

		public static T DeserializeFromJson<T>(this HttpResponseMessage message)
		{
			using (var sr = new StreamReader(message.GetResponseStream()))
			{
				var response = sr.ReadToEnd();

				return JsonConvert.DeserializeObject<T>(response);
			}
		}

		public static T DeserializeFromXml<T>(this HttpResponseMessage message)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = message.GetResponseStream())
			{
				return (T)serializer.ReadObject(stream);
			}
		}

		public static bool IsJsonContent(this HttpResponseMessage message)
		{
			return message.ContentType.StartsWith( MediaType.Json );
		}

		public static bool IsXmlContent(this HttpResponseMessage message)
		{
			return message.ContentType.StartsWith( MediaType.Xml );
		}

		public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
		{
			var tcs = new TaskCompletionSource<Stream>();

			try
			{
				request.BeginGetRequestStream(iar =>
				{
					try
					{
						var response = request.EndGetRequestStream(iar);
						tcs.SetResult(response);
					}
					catch (Exception exc)
					{
						tcs.SetException(exc);
					}
				}, null);
			}
			catch (Exception exc)
			{
				tcs.SetException(exc);
			}

			return tcs.Task;
		}

		public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
		{
			var tcs = new TaskCompletionSource<HttpWebResponse>();

			try
			{
				request.BeginGetResponse(iar =>
				{
					try
					{
						var response = (HttpWebResponse)request.EndGetResponse(iar);
						tcs.SetResult(response);
					}
					catch (Exception exc)
					{
						tcs.SetException(exc);
					}
				}, null);
			}
			catch (Exception exc)
			{
				tcs.SetException(exc);
			}

			return tcs.Task;
		}
	}
}