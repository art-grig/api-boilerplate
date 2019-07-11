using System;
using System.Collections.Generic;
using System.Text;
using WI.ApiBoilerplate.ORM.Repositories;
using WI.ApiBoilerplate.ORM.UnitOfWork;

namespace WI.ApiBoilerplate.ORM.DI
{
	public class RepositoryDIConfigurator : IDependencyInjectionConfigurator
	{
		public void RegisterServices(Action<Type, Type> action)
		{
			action(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
			action(typeof(IUnitOfWork), typeof(ApplicationUnitOfWork));
		}
	}
}
