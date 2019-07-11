using WI.ApiBoilerplate.ORM.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.ORM
{
	public class ApplicationUnitOfWork : EfUnitOfWork<ApplicationDbContext>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="WebImpactCentralUnitOfWork"/>.
		/// </summary>
		/// <param name="dbContext"></param>
		public ApplicationUnitOfWork(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}
