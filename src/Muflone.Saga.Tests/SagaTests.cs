using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muflone.Core;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga.Tests.Persistence;
using Xunit;

namespace Muflone.Saga.Tests
{
	public class SagaTests
	{
		private SagaToTest saga;
		private InProcessServiceBus serviceBus;
		private InMemorySagaRepository inMemorySagaRepository = new InMemorySagaRepository(new Serializer());

		public SagaTests()
		{
			saga = new SagaToTest(serviceBus, inMemorySagaRepository);
			serviceBus = new InProcessServiceBus(typeof(FakeStartingCommand),
				new Dictionary<Type, Event>()
					{ { typeof(FakeStep2Command), new FakeResponse(new MyDomainId(Guid.NewGuid()), correlationId, "zxc") } });
		}

		public class MyDomainId : IDomainId
		{
			public Guid Value { get; }

			public MyDomainId(Guid value)
			{
				Value = value;
			}
		}

		private Guid correlationId = Guid.NewGuid();

		[Fact]
		public async Task Saga_StartsWithCommand()
		{
			var command = new FakeStartingCommand(new MyDomainId(Guid.NewGuid()), correlationId, "abc");
			await serviceBus.SendAsync(command);

			var commands = serviceBus.SentCommands();
			Assert.Equal(2, commands.Count);
			Assert.IsType<FakeStep2Command>(commands[1]);
			Assert.Equal("abc", ((FakeStep2Command)commands[1]).Value1);

			var data = await inMemorySagaRepository.GetById<SagaToTest.MyData>(correlationId);
			Assert.NotNull(data);
			Assert.Equal("abc", data.Value1);
			Assert.Equal("qwe", data.Value2);
		}
	}
}