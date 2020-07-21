using System;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaRepository
	{
		Task<TSagaState> GetById<TSagaState>(Guid id) where TSagaState : class, new();
		Task Save<TSagaState>(Guid correlationId, TSagaState sagaState) where TSagaState : class, new();
		Task Complete(Guid correlationId);
	}
}