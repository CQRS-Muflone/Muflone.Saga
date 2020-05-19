using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	//TODO: Implement .ConfigureAwaiter(false) to be safe
	public class SagaRepository : ISagaRepository, IDisposable
	{
		public Task<TSaga> GetById<TSaga>(ISagaId id) where TSaga : class, ISaga
		{
      //Get where(sagaId==id)

      //Returns null if not found
      //Returns Saga if exists
      //Returns Saga if exists and completed = true
      
      throw new NotImplementedException();
		}

		public Task Save(ISaga saga, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
		{
			throw new NotImplementedException();
		}

		public Task Save(ISaga saga, Guid commitId)
		{
			throw new NotImplementedException();
		}

		public Task Complete<TSaga>(TSaga saga) where TSaga : class, ISaga
		{
			return Complete(saga.Id);
		}

		public Task Complete(ISagaId id)
		{
      //Saga.completed = true

			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}