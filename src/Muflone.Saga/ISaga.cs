using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using System;
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
	public interface ISagaStartedByAsync<in TCommand> : IDisposable where TCommand : Command
	{
		Task StartedByAsync(TCommand command);
	}

	//Could be a DomainEvent or an IntegrationEvent
	public interface ISagaEventHandlerAsync<in TEvent> : IDisposable where TEvent : Event
	{
		Task HandleAsync(TEvent @event);
	}
}