using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.ORM
{
	//TODO move to foundation
	public interface IDependencyInjectionConfigurator
	{
		void RegisterServices(Action<Type, Type> action);
	}
}
