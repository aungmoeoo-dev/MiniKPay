using Microsoft.Data.SqlClient;

namespace MiniKPay.RestApi
{
	public class AppSettings
	{
		private static readonly SqlConnectionStringBuilder ConnectionStringBuilder = new()
		{
			DataSource = ".",
			InitialCatalog = "MiniKPay",
			UserID = "sa",
			Password = "Aa145156167!",
			TrustServerCertificate = true
		};

		public static readonly string ConnectionString = ConnectionStringBuilder.ToString();
	}
}
