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

		public TestSaga(IServiceBus serviceBus, ISagaRepository<MyData> repository) : base(serviceBus, repository)
		{
		}

		public async Task StartedBy(FakeCommand command)
		{
			throw new NotImplementedException();
			
			//var data = new MyData(){Value1 = "abc", Value2 = "qwe"};
			//serviceBus.Send(new Command())
			//await Repository.Save(command.Headers.CorrelationId, data);
		}

		public Task Handle(FakeResponseEvent @event)
		{
			throw new NotImplementedException();

			//var data = await Repository.GetById(@event.Headers.CorrelationId);
			//my logic
			//serviceBus.Send(new Command())
			//await Repository.Save(@event.Headers.CorrelationId, data);
			// or
			//await Repository.Complete(@event.Headers.CorrelationId);
		}


		public Task Handle(FakeResponseErrorEvent @event)
		{
			throw new NotImplementedException();
		}
	}
}
