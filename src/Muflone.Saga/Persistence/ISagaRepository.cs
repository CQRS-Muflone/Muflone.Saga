using System;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	//TODO: Is it possible to create a Repo agnostic to TSagaState? Make sense at all?
	public interface ISagaRepository<TSagaState> where TSagaState : class, new()
	{
		Task<TSagaState> GetById(Guid id);
		//Task Save(Guid id, TSagaState sagaState, IDictionary<string, object> updateHeaders);
		Task Save(Guid id, TSagaState sagaState);
		Task Complete(Guid id);
	}

}