using System.Collections.Generic;
using System.Threading.Tasks;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Muflone.Saga
{
	public interface ISaga
	{
		ISagaId Id { get; }
		IDictionary<string, object> Headers { get; set; }
	}

	public interface ISaga<TSagaData> : ISaga where TSagaData : class
	{
		TSagaData SagaData { get; set; }
	}
	
	public interface ISagaMessage
	{
		ISagaId SagaId { get; set; }
	}

	//A command starts the saga
	public interface IStartedBy<in TCommand> : ISagaMessage where TCommand : Command
	{
		Task StartedBy(TCommand command);
	}

	//Could be a DomainEvent or an IntegrationEvent
	public interface IEventHandler<in TEvent> where TEvent : IEvent
	{
		Task Handle(TEvent command);
	}
}