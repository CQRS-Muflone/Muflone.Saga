using System;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaFactory
	{
		//Task<ISaga> Build(Type type, string id);
		Task<T> Build<T>(string serializedSaga) where T : class, ISaga;
	}
}