namespace Zebble.Billing
{
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;

	public static class StreamExtensions
	{
		public static async Task<string> ReadAsString(this Stream stream)
		{
			using var reader = new StreamReader(stream, Encoding.UTF8);

			return await reader.ReadToEndAsync();
		}

		public static async Task<T> ConvertTo<T>(this Stream stream)
		{
			return (await stream.ReadAsString()).FromJson<T>();
		}
	}
}
