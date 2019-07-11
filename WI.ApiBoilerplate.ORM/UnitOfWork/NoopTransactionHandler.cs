using System;

namespace WI.ApiBoilerplate.ORM.UnitOfWork
{
	/// <summary>
	/// No-op implementation of the <see cref="ITransactionHandler"/>.
	/// </summary>
	public class NoopTransactionHandler : ITransactionHandler
	{
		/// <inheritdoc />
		public void Commit()
		{
			// No-op.
		}

		/// <inheritdoc />
		public void Dispose()
		{
			// No-op.
		}
	}
}
