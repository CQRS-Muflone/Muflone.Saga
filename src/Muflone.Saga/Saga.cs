using Microsoft.Extensions.Logging;
using Muflone.Messages.Commands;
using Muflone.Persistence;
using Muflone.Saga.Persistence;
using System;
using System.Threading.Tasks;

namespace Muflone.Saga;

public abstract class Saga<TCommand, TSagaState> :
	ISagaStartedByAsync<TCommand>,
	ISaga<TSagaState>
	where TCommand : Command
	where TSagaState : class, new()
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

	public abstract Task StartedByAsync(TCommand command);

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