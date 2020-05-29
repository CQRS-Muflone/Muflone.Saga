using System;
using System.Threading.Tasks;
using Muflone.Core;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public class FakeCommand : Command
	{
		public string Value1 { get; }

		public FakeCommand(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}

	public class FakeResponseEvent : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponseEvent(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}
	public class FakeResponseErrorEvent : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponseErrorEvent(IDomainId aggregateId, Guid correlationId, string value1, string who = "anonymous") : base(aggregateId, correlationId, who)
		{
			Value1 = value1;
		}
	}

	public class TestSaga : Saga<TestSaga.MyData>, IStartedBy<FakeCommand>, IEventHandler<FakeResponseEvent>, IEventHandler<FakeResponseErrorEvent>
	{
		public class MyData
		{
			public string Value1;
			public string Value2;
		}

		public TestSaga(IServiceBus serviceBus, ISagaRepository repository) : base(serviceBus, repository)
		{
		}

		public async Task StartedBy(FakeCommand command)
		{
			throw new NotImplementedException();

			//var data = new MyData() { Value1 = "abc", Value2 = "qwe" };
			//await Repository.Save(command.Headers.CorrelationId, data);
			//await ServiceBus.Send(new Command());
		}

		public async Task Handle(FakeResponseEvent @event)
		{
			throw new NotImplementedException();

			//var data = await Repository.GetById<MyData>(@event.Headers.CorrelationId);
			////my logic
			//await Repository.Save(@event.Headers.CorrelationId, data);
			
			//await ServiceBus.Send(new Command());
			////or
			//await Repository.Complete(@event.Headers.CorrelationId);
		}


		public Task Handle(FakeResponseErrorEvent @event)
		{
			throw new NotImplementedException();
			//revert strategies
		}
	}
}
