using Microsoft.EntityFrameworkCore;
using MiniKPay.RestApi.Features.Transaction;
using MiniKPay.RestApi.Features.User;

namespace MiniKPay.RestApi;

public class AppDbContext : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(AppSettings.ConnectionString);
	}

	public DbSet<UserModel> Users { get; set; }
	public DbSet<TransactionHistoryModel> Transactions { get; set; }
}