using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga.Persistence;
using System;
using System.Threading.Tasks;

namespace Muflone.Saga
{
	public abstract class SagaEventHandlerAsync<TEvent> : ISagaEventHandlerAsync<TEvent> where TEvent : Event
	{
		protected readonly IServiceBus ServiceBus;
		protected readonly ISagaRepository Repository;

		protected SagaEventHandlerAsync(IServiceBus serviceBus, ISagaRepository repository)
		{
			ServiceBus = serviceBus;
			Repository = repository;
		}

		public abstract Task HandleAsync(TEvent @event);

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

		~SagaEventHandlerAsync()
		{
			Dispose(false);
		}


		#endregion
	}
}