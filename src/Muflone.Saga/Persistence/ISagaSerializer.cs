using System;
using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISagaSerializer
	{
		Task<TSagaState> Deserialize<TSagaState>(string serializedState) where TSagaState : class, new();
		Task<string> Serialize<TSagaState>(TSagaState state) where TSagaState : class, new();
	}
}