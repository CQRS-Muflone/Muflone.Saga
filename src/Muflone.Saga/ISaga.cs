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
	public interface IStartedBy<in TCommand> where TCommand : ICommand
	{
		Task StartedBy(TCommand command);
	}

	//Could be a DomainEvent or an IntegrationEvent
	public interface IEventHandler<in TEvent> where TEvent : IEvent
	{
		Task Handle(TEvent @event);
	}
}