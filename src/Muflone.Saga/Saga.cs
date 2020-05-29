using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class Saga<TSagaState> : ISaga<TSagaState> where TSagaState : class, new()
	{
		protected readonly IServiceBus ServiceBus;

		protected readonly ISagaRepository Repository;
		public TSagaState SagaState { get; set; }

		protected Saga(IServiceBus serviceBus, ISagaRepository repository)
		{
			ServiceBus = serviceBus;
			Repository = repository;
		}
	}
}
