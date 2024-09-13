using Muflone.Core;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga.Tests.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Muflone.Saga.Tests
{
	public class SagaTests
	{
		private SagaToTest _saga;
		private InProcessServiceBus _serviceBus;
		private InMemorySagaRepository _inMemorySagaRepository = new(new Serializer());

		public SagaTests()
		{
			_saga = new SagaToTest(_serviceBus, _inMemorySagaRepository);
			_serviceBus = new InProcessServiceBus(typeof(FakeStartingCommand),
				new Dictionary<Type, Event>()
					{ { typeof(FakeStep2Command), new FakeResponse(new MyDomainId(Guid.NewGuid().ToString()), _correlationId, "zxc") } });
		}

		public class MyDomainId : IDomainId
		{
			public string Value { get; }

			public MyDomainId(string value)
			{
				Value = value;
			}
		}

		private readonly Guid _correlationId = Guid.NewGuid();

		[Fact]
		public async Task Saga_StartsWithCommand()
		{
			var command = new FakeStartingCommand(new MyDomainId(Guid.NewGuid().ToString()), _correlationId, "abc");
			await _serviceBus.SendAsync(command);

			var commands = _serviceBus.SentCommands();
			Assert.Equal(2, commands.Count);
			Assert.IsType<FakeStep2Command>(commands[1]);
			Assert.Equal("abc", ((FakeStep2Command)commands[1]).Value1);

			var data = await _inMemorySagaRepository.GetByIdAsync<SagaToTest.MyData>(_correlationId);
			Assert.NotNull(data);
			Assert.Equal("abc", data.Value1);
			Assert.Equal("qwe", data.Value2);
		}
	}
}