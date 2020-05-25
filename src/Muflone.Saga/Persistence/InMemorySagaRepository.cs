using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

//TODO: To implement in the persistence concrete code. Create also a package for MongoDB or RavenDB as an example?

namespace Muflone.Saga.Persistence
{
	//TODO: Implement .ConfigureAwaiter(false) to be safe
	public class InMemorySagaRepository<TSagaState> : ISagaRepository<TSagaState>, IDisposable where TSagaState : class, new()
	{
		private readonly ISagaSerializer serializer;
		internal static readonly ConcurrentDictionary<ISagaId, string> Data = new ConcurrentDictionary<ISagaId, string>();
		internal static readonly ConcurrentDictionary<ISagaId, string> Headers = new ConcurrentDictionary<ISagaId, string>();

		public InMemorySagaRepository(ISagaSerializer serializer)
		{
			this.serializer = serializer;
		}

		public async Task<TSagaState> GetById(ISagaId id)
		{
			if (!Data.TryGetValue(id, out var stateSerialized))
				return default;

			var headers = new ConcurrentDictionary<string, object>();
			if (Headers.TryGetValue(id, out var headersSerialized))
			{
				headers = await serializer.Deserialize<ConcurrentDictionary<string, object>>(headersSerialized);
			}

			var state = await serializer.Deserialize<TSagaState>(stateSerialized);
			return state;
		}

		public Task Save(TSagaState sagaState)
		{
			return Save(sagaState, null);
		}

		public Task Save(TSagaState sagaState, IDictionary<string, object> updateHeaders)
		{
			throw new NotImplementedException();
		}

		public Task Complete(TSagaState sagaState)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}