using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

//TODO: To implement in the persistence concrete code. Create also a package for MongoDB or RavenDB as an example?

namespace Muflone.Saga.Persistence
{
	//TODO: Implement .ConfigureAwaiter(false) to be safe
	public class InMemorySagaRepository : ISagaRepository, IDisposable
	{
		internal static readonly ConcurrentDictionary<ISagaId, string> Data = new ConcurrentDictionary<ISagaId, string>();
		internal static readonly ConcurrentDictionary<ISagaId, string> Headers = new ConcurrentDictionary<ISagaId, string>();

		private readonly ISagaFactory factory;

		public InMemorySagaRepository(ISagaFactory factory)
		{
			this.factory = factory;
		}
		
		public Task<T> GetById<T>(ISagaId id) where T : class, ISaga
		{
			//TODO implement a ISerializer in the constructor or extend the SagaFactory?

			if (!Data.TryGetValue(id, out var sagaSerialized))
				return Task.FromResult(default(T));

			var headers = new Dictionary<string, object>();
			if (Headers.TryGetValue(id, out var headersSerialized))
			{
				
				//headers = messageSerialiser.Deserialise<Dictionary<string, string>>(headersSerialized);
				headers = JsonConvert.DeserializeObject<Dictionary<string, object>>(headersSerialized);
			}
			
			//var sagaDataType = NSagaReflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));
			//var dataObject = messageSerialiser.Deserialise(dataSerialised, sagaDataType);
			//var saga = sagaFactory.ResolveSaga<TSaga>();
			//NSagaReflection.Set(saga, "SagaData", dataObject);
			//NSagaReflection.Set(saga, "CorrelationId", correlationId);
			//NSagaReflection.Set(saga, "Headers", headers);

			//return saga;

			return factory.Build<T>(sagaSerialized);
		}

		public Task Save<T>(T saga, IDictionary<string, object> updateHeaders) where T : class, ISaga
		{
			throw new NotImplementedException();
		}

		public Task Save<T>(T saga) where T : class, ISaga
		{
			throw new NotImplementedException();
		}

		public Task Complete<T>(T saga) where T : class, ISaga
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}