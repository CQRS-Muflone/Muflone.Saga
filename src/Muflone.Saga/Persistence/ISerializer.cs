using System.Threading.Tasks;

namespace Muflone.Saga.Persistence
{
	public interface ISerializer
	{
		Task<T> Deserialize<T>(string serializedData) where T : class, new();
		Task<string> Serialize<T>(T data);
	}
}