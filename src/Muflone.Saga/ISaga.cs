using System;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using System.Threading.Tasks;

namespace Muflone.Saga
{
	public interface ISaga : IDisposable
	{
	}

	public interface ISaga<TSagaState> : ISaga where TSagaState : class
	{
		TSagaState SagaState { get; set; }
	}

	//A command starts the saga
	public interface ISagaStartedBy<in TCommand> : IDisposable where TCommand : Command
	{
		Task StartedBy(TCommand command);
	}

	//Could be a DomainEvent or an IntegrationEvent
	public interface ISagaEventHandler<in TEvent> : IDisposable where TEvent : Event
	{
		Task Handle(TEvent @event);
	}
}