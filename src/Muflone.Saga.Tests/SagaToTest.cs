using Muflone.Core;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga.Persistence;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Muflone.Saga.Tests
{
	public class FakeStartingCommand : Command
	{
		public string Value1 { get; }

		public FakeStartingCommand(IDomainId aggregateId, Guid correlationId, string value1) : base(aggregateId)
		{
			Value1 = value1;
			UserProperties[HeadersNames.CorrelationId] = correlationId.ToString();
		}
	}

	public class FakeStep2Command : Command
	{
		public string Value1 { get; }

		public FakeStep2Command(IDomainId aggregateId, Guid correlationId, string value1) : base(aggregateId)
		{
			Value1 = value1;
			UserProperties[HeadersNames.CorrelationId] = correlationId.ToString();
		}
	}

	public class FakeResponse : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponse(IDomainId aggregateId, Guid correlationId, string value1) : base(aggregateId)
		{
			Value1 = value1;
			UserProperties[HeadersNames.CorrelationId] = correlationId.ToString();
		}
	}
	public class FakeResponseError : DomainEvent
	{
		public string Value1 { get; }

		public FakeResponseError(IDomainId aggregateId, Guid correlationId, string value1) : base(aggregateId)
		{
			Value1 = value1;
			UserProperties[HeadersNames.CorrelationId] = correlationId.ToString();
		}
	}

	public class SagaToTest : Saga<FakeStartingCommand, SagaToTest.MyData>,  ISagaEventHandlerAsync<FakeResponse>, ISagaEventHandlerAsync<FakeResponseError>
	{
		public class MyData
		{
			public string Value1;
			public string Value2;
		}

		public SagaToTest(IServiceBus serviceBus, ISagaRepository repository) : base(serviceBus, repository, new NullLoggerFactory())
		{
		}

		public override async Task StartedByAsync(FakeStartingCommand command)
		{
			var data = new MyData() { Value1 = command.Value1, Value2 = "qwe" };
			await Repository.SaveAsync(Guid.Parse(command.UserProperties[HeadersNames.CorrelationId].ToString() ?? string.Empty), data);
			await ServiceBus.SendAsync(new FakeStep2Command(command.AggregateId, Guid.Parse(command.UserProperties[HeadersNames.CorrelationId].ToString() ?? string.Empty), data.Value1));
		}

		public async Task HandleAsync(FakeResponse @event)
		{
			//var data = await Repository.GetById<MyData>(@event.Headers.CorrelationId);
			//await Repository.Save(@event.Headers.CorrelationId, data);

			await Repository.CompleteAsync(@event.Headers.CorrelationId);
		}

		public Task HandleAsync(FakeResponseError @event)
		{
			//revert strategies
			throw new NotImplementedException();
		}
	}
}
