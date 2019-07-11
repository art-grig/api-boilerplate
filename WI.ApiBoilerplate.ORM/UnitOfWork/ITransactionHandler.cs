using System;

namespace WI.ApiBoilerplate.ORM.UnitOfWork
{
	/// <summary>
	/// Represents the abstration over storage transaction.
	/// </summary>
	public interface ITransactionHandler : IDisposable
	{
		/// <summary>
		/// Commits current transaction.
		/// </summary>
		void Commit();
	}
}
