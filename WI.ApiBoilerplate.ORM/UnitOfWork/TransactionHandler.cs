using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace WI.ApiBoilerplate.ORM.UnitOfWork
{
	/// <summary>
	/// Default implementation of the <see cref="ITransactionHandler"/>.
	/// </summary>
	public class TransactionHandler : ITransactionHandler
	{
		private readonly IDbContextTransaction _transaction;
		private bool _disposed;

		/// <summary>
		/// Initializes a new instanse of <see cref="TransactionHandler"/>.
		/// </summary>
		/// <param name="transaction">An instanse of <see cref="IDbContextTransaction"/>.</param>
		public TransactionHandler(IDbContextTransaction transaction)
		{
			_transaction = transaction;
		}

		/// <inheritdoc />
		public void Commit()
		{
			ThrowIfDisposed();
			_transaction.Commit();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_transaction.Dispose();
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("TransactionHandler was already disposed.");
			}
		}
	}
}
