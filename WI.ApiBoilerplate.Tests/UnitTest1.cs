using System;
using System.Threading.Tasks;
using WI.ApiBoilerplate.ORM;
using WI.ApiBoilerplate.ORM.UnitOfWork;
using WI.ApiBoilerplate.Tests.Infrastructure;
using Xunit;

namespace WI.ApiBoilerplate.Tests
{
	public class UnitTest1 : TestBase
	{
		[Fact]
		public async Task Test1()
		{
			var uow = new ApplicationUnitOfWork(DbContext);
			Assert.True(DbContext.Database.CurrentTransaction == null);

			using (var transaction = await uow.EnsureTransactionAsync())
			{
				Assert.False(DbContext.Database.CurrentTransaction == null);
				Assert.IsType<TransactionHandler>(transaction);

				using (var transaction2 = await uow.EnsureTransactionAsync())
				{
					Assert.False(DbContext.Database.CurrentTransaction == null);
					Assert.IsType<NoopTransactionHandler>(transaction2);
				}

				using (var transaction3 = await uow.EnsureTransactionAsync())
				{
					Assert.False(DbContext.Database.CurrentTransaction == null);
					Assert.IsType<NoopTransactionHandler>(transaction3);
				}
			}

			Assert.True(DbContext.Database.CurrentTransaction == null);
		}

		protected override Task SeedAsync()
		{
			return Task.CompletedTask;
		}
	}
}
