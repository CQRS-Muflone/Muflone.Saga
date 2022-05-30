using System;
using System.Threading.Tasks;
using Muflone.Messages.Events;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class SagaEventHandler<TEvent> : ISagaEventHandler<TEvent> where TEvent : Event
	{
		protected readonly IServiceBus ServiceBus;
		protected readonly ISagaRepository Repository;

		protected SagaEventHandler(IServiceBus serviceBus, ISagaRepository repository)
		{
			this.ServiceBus = serviceBus;
			this.Repository = repository;
		}

		public abstract Task Handle(TEvent @event);

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

		~SagaEventHandler()
		{
			Dispose(false);
		}


		#endregion
	}
}