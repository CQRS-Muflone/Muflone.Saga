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
		
		protected FakeCommand(IDomainId aggregateId, string value1) : base(aggregateId)
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

	public class TestSaga : Saga<TestSaga.MyData>, IStartedBy<FakeCommand>, IEventHandler<FakeResponseEvent>
	{
		public class MyData
		{
			private string value1;
			private string value2;
		}

		public TestSaga(ISagaRepository<MyData> repository) : base(repository)
		{
		}

		public async Task StartedBy(FakeCommand command)
		{
			//throw new NotImplementedException();
			var data = await Repository.GetById(command.Headers.CorrelationId);

			await Repository.Save(command.Headers.CorrelationId, data);
		}

		public Task Handle(FakeResponseEvent @event)
		{
			//@event.Headers.CorrelationId
			throw new NotImplementedException();
		}


		
	}
}
