using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaRepository
	{
		Task<T> GetById<T>(ISagaId id) where T : class, ISaga;

		Task Save<T>(T saga, IDictionary<string, object> updateHeaders) where T : class, ISaga;
		Task Save<T>(T saga) where T : class, ISaga;
		Task Complete<T>(T saga) where T : class, ISaga;
	}

}