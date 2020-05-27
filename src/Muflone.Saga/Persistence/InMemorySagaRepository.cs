using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

//TODO: To implement in the persistence concrete code. Create also a package for MongoDB or RavenDB as an example?
namespace Muflone.Saga.Persistence
{
	public class InMemorySagaRepository<TSagaState> : ISagaRepository<TSagaState>, IDisposable where TSagaState : class, new()
	{
		private readonly ISerializer serializer;
		internal static readonly ConcurrentDictionary<Guid, string> Data = new ConcurrentDictionary<Guid, string>();
		//internal static readonly ConcurrentDictionary<Guid, string> Headers = new ConcurrentDictionary<Guid, string>();

		public InMemorySagaRepository(ISerializer serializer)
		{
			this.serializer = serializer;
		}

		public async Task<TSagaState> GetById(Guid id)
		{
			if (!Data.TryGetValue(id, out var stateSerialized))
				return default;

			//var headers = new ConcurrentDictionary<string, object>();
			//if (Headers.TryGetValue(id, out var headersSerialized))
			//	headers = await serializer.Deserialize<ConcurrentDictionary<string, object>>(headersSerialized);
			
			return await serializer.Deserialize<TSagaState>(stateSerialized).ConfigureAwait(false);
		}

		//public Task Save(Guid id, TSagaState sagaState)
		//{
		//	return Save(id, sagaState, null);
		//}

		public async Task Save(Guid id, TSagaState sagaState /*, IDictionary<string, object> updateHeaders*/)
		{
			var serializedData = await serializer.Serialize(sagaState);
			//var serializedHeaders = await serializer.Serialize(updateHeaders);

			Data[id] = serializedData;
			//Headers[id] = serializedHeaders;
		}

		public Task Complete(Guid id)
		{
			Data.TryRemove(id, out _);
			//Headers.TryRemove(id, out _);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			Data.Clear();
			//Headers.Clear();
		}
	}
}