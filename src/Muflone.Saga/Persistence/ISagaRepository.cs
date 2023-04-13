using System;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaRepository
	{
		Task<TSagaState> GetByIdAsync<TSagaState>(Guid id) where TSagaState : class, new();
		Task SaveAsync<TSagaState>(Guid correlationId, TSagaState sagaState) where TSagaState : class, new();
		Task CompleteAsync(Guid correlationId);
	}
}