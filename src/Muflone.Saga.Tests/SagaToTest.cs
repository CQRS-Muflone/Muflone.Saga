using System;
using System.Threading.Tasks;
using Muflone.Core;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Saga.Persistence;

namespace Muflone.Saga.Tests
{
	public class FakeStartingCommand : Command
	{
		public string Value1 { get; }

		public FakeStartingCommand(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}

	public class FakeStep2Command : Command
	{
		public string Value1 { get; }

		public FakeStep2Command(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}

	public class FakeResponse : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponse(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}
	public class FakeResponseError : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponseError(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}

	public class SagaToTest : Saga<SagaToTest.MyData>, ISagaStartedBy<FakeStartingCommand>, ISagaEventHandler<FakeResponse>, ISagaEventHandler<FakeResponseError>
	{
		public class MyData
		{
			public string Value1;
			public string Value2;
		}

		public SagaToTest(IServiceBus serviceBus, ISagaRepository repository) : base(serviceBus, repository)
		{
		}

		public async Task StartedBy(FakeStartingCommand command)
		{
			var data = new MyData() { Value1 = command.Value1, Value2 = "qwe" };
			await Repository.Save(command.Headers.CorrelationId, data);
			await ServiceBus.Send(new FakeStep2Command(command.AggregateId, command.Headers.CorrelationId, data.Value1, command.Who));
		}

		public async Task Handle(FakeResponse @event)
		{
			//var data = await Repository.GetById<MyData>(@event.Headers.CorrelationId);
			//await Repository.Save(@event.Headers.CorrelationId, data);

			await Repository.Complete(@event.Headers.CorrelationId);
		}

		public Task Handle(FakeResponseError @event)
		{
			//revert strategies
			throw new NotImplementedException();
		}
	}
}
