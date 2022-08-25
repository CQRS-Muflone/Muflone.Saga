using System;
using System.Threading.Tasks;
using Muflone.Messages.Commands;
using Muflone.Persistence;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class SagaStartedByHandler<TCommand, TSagaState> : ISagaStartedBy<TCommand> where TCommand : Command
			where TSagaState : class
	{
		protected readonly IServiceBus ServiceBus;
		protected readonly ISagaRepository Repository;

		protected SagaStartedByHandler(IServiceBus serviceBus, ISagaRepository repository)
		{
			this.ServiceBus = serviceBus;
			this.Repository = repository;
		}

		public abstract Task StartedBy(TCommand command);

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

		~SagaStartedByHandler()
		{
			Dispose(false);
		}


		#endregion
	}
}