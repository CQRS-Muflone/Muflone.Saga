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

		public FakeResponseEvent(IDomainId aggregateId, string value1, string who = "anonymous") : base(aggregateId, who)
		{
			Value1 = value1;
		}
	}

	public class TestSaga : Saga<TestSaga.MyData>, IStartedBy<FakeCommand>, IEventHandler<FakeResponseEvent>
	{
		public ISagaId CorrelationId { get; set; }

		public class MyData
		{
			private string value1;
			private string value2;
		}

		
		public async Task StartedBy(FakeCommand command)
		{
			var data = await Repository.GetById(base.CorrelationId);

			

			await Repository.Save(data);
		}

		public Task Handle(FakeResponseEvent command)
		{
			

			throw new NotImplementedException();
		}


		public TestSaga(ISagaRepository<MyData> repository) : base(repository)
		{
		}
	}
}
