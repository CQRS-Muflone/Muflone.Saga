using System;
using Microsoft.Extensions.Logging;
using Muflone.Persistence;
using Muflone.Saga.Persistence;

namespace Muflone.Saga;

public abstract class Saga<TSagaState> : ISaga<TSagaState> where TSagaState : class, new()
{
	protected readonly IServiceBus ServiceBus;
	protected readonly ISagaRepository Repository;
	protected readonly ILoggerFactory LoggerFactory;
	public TSagaState SagaState { get; set; }

	protected Saga(IServiceBus serviceBus, ISagaRepository repository, ILoggerFactory loggerFactory)
	{
		ServiceBus = serviceBus;
		Repository = repository;
		LoggerFactory = loggerFactory;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Saga()
	{
		Dispose(false);
	}
}