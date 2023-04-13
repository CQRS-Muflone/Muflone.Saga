using Muflone.Messages.Commands;
using Muflone.Persistence;
using Muflone.Saga.Persistence;
using System;
using System.Threading.Tasks;

namespace Muflone.Saga
{
	public abstract class SagaStartedByHandlerAsync<TCommand, TSagaState> : ISagaStartedByAsync<TCommand> where TCommand : Command
			where TSagaState : class
	{
		protected readonly IServiceBus ServiceBus;
		protected readonly ISagaRepository Repository;

		protected SagaStartedByHandlerAsync(IServiceBus serviceBus, ISagaRepository repository)
		{
			ServiceBus = serviceBus;
			Repository = repository;
		}

		public abstract Task StartedByAsync(TCommand command);

		#region Dispose
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

		~SagaStartedByHandlerAsync()
		{
			Dispose(false);
		}


		#endregion
	}
}