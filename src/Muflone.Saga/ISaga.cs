using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using System.Threading.Tasks;

namespace Muflone.Saga
{
	public interface ISaga
	{
	}

	public interface ISaga<TSagaState> : ISaga where TSagaState : class
	{
		TSagaState SagaState { get; set; }
	}

	//A command starts the saga
	public interface ISagaStartedBy<in TCommand> where TCommand : Command
	{
		Task StartedBy(TCommand command);
	}

	//Could be a DomainEvent or an IntegrationEvent
	public interface ISagaEventHandler<in TEvent> where TEvent : Event
	{
		Task Handle(TEvent @event);
	}
}