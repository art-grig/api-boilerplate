using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.ORM.UnitOfWork
{
	/// <summary>
	/// EF implementation of the <see cref="IUnitOfWork"/>.
	/// </summary>
	/// <typeparam name="TContext">Type of EF dbcontext.</typeparam>
	public class EfUnitOfWork<TContext> : IUnitOfWork
		where TContext: DbContext
	{
		private readonly TContext _dbContext;
		private ITransactionHandler _transactionHandler;

		/// <summary>
		/// Initializes a new instance of <see cref="EfUnitOfWork{TContext}"/>.
		/// </summary>
		/// <param name="dbContext">Instance of <see cref="DbContext"/>.</param>
		public EfUnitOfWork(TContext dbContext)
		{
			_dbContext = dbContext;
		}

		/// <inheritdoc />
		public virtual Task<ITransactionHandler> EnsureTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return EnsureTransactionAsync(IsolationLevel.Unspecified, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<ITransactionHandler> EnsureTransactionAsync(IsolationLevel isolationLevel,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			IDbContextTransaction currentTransaction = _dbContext.Database.CurrentTransaction;
			if (currentTransaction == null)
			{
				currentTransaction =  await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
			}
			else
			{
				var currentIsolationLevel = currentTransaction.GetDbTransaction().IsolationLevel;
				if (isolationLevel != IsolationLevel.Unspecified && currentIsolationLevel != isolationLevel)
				{
					throw new InvalidOperationException(
						$"There is already opened transaction [isolation level = {currentIsolationLevel.ToString()}]. " +
						$"The transaction isolation level cannot be changed.");
				}
			}

			if (_transactionHandler == null)
			{
				_transactionHandler = new TransactionHandler(currentTransaction);
				return _transactionHandler;
			}

			return new NoopTransactionHandler();
		}

		/// <inheritdoc />
		public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _dbContext.SaveChangesAsync(cancellationToken);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_transactionHandler?.Dispose();
			_transactionHandler = null;
		}
	}
}
