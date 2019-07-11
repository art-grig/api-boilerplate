using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.ORM.UnitOfWork
{
	/// <summary>
	/// Represents a unit of work pattern with transaction support.
	/// </summary>
	public interface IUnitOfWork: IDisposable
	{
		/// <summary>
		/// Opens a new transaction and returns a new TransactionHandler if there wasn't opened transaction
		/// or if one transaction is already opened this methos return the transaction handler wrapper that do nothing.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the instance of <see cref="ITransactionHandler"/>.
		/// </returns>
		Task<ITransactionHandler> EnsureTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Opens a new transaction and returns a new TransactionHandler if there wasn't opened transaction
		/// or if one transaction is already opened this methos return the transaction handler wrapper that do nothing.
		/// </summary>
		/// <remarks>
		/// If there is already opened transaction has the same isolation level as specified,
		/// <paramref name="isolationLevel"/> will no effect.
		/// If there is already opened transaction has different isolation level than specified by
		/// <paramref name="isolationLevel"/> exception will be thrown.
		/// </remarks>
		/// <param name="isolationLevel">The transaction isolation  level.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the instance of <see cref="ITransactionHandler"/>.
		/// </returns>
		Task<ITransactionHandler> EnsureTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Asynchronously saves all changes made in this unit of work to the database.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
		/// <returns>
		/// A task that represents the asynchronous save operation. The task result contains the
		/// number of state entries written to the database.
		/// </returns>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
