using AutoMapper;
using WI.ApiBoilerplate.ORM.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.Services
{
	public abstract class BaseService
	{
		private readonly IServiceProvider _serviceProvider;

		protected readonly IUnitOfWork _uow;
		protected readonly IMapper _mapper;

		public BaseService(IServiceProvider serviceProvider, IUnitOfWork uow, IMapper mapper) 
		{
			_serviceProvider = serviceProvider;
			_uow = uow;
			_mapper = mapper;
		}
	}
}
