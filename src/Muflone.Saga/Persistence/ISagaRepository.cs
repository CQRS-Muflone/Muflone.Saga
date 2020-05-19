using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaRepository
	{
		Task<TSaga> GetById<TSaga>(ISagaId id) where TSaga : class, ISaga;

		Task Save(ISaga saga, Guid commitId, Action<IDictionary<string, object>> updateHeaders);
		Task Save(ISaga saga, Guid commitId);

		//TODO Logical delete
		/// <summary>
		/// Delete saga from storage
		/// </summary>
		Task Complete<TSaga>(TSaga saga) where TSaga : class, ISaga;

		/// <summary>
		/// Delete saga from storage
		/// </summary>
		Task Complete(ISagaId id);
	}

}