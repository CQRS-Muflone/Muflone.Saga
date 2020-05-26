using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class Saga<TSagaState> : ISaga<TSagaState> where TSagaState : class, new()
	{
		protected readonly ISagaRepository<TSagaState> Repository;
		//public IDictionary<string, object> Headers { get; set; }
		public TSagaState SagaState { get; set; }

		protected Saga(ISagaRepository<TSagaState> repository)
		{
			Repository = repository;
		}
	}
}
