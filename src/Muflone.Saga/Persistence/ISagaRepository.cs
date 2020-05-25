using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaRepository<TSagaState> where TSagaState : class, new()
	{
		//TODO: Is it possible to create a Repo agnostic to TSagaState? Make sense at all?
		Task<TSagaState> GetById(ISagaId id);
		Task Save(TSagaState sagaState, IDictionary<string, object> updateHeaders);
		Task Save(TSagaState sagaState);
		Task Complete(TSagaState sagaState);
	}

}