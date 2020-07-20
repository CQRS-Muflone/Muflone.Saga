using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Muflone.Saga.Tests.Persistence
{
	public class InProcessServiceBus : IServiceBus
	{
		private readonly Type _startingCommand;
		private readonly Dictionary<Type, Event> _events;
		//private readonly Dictionary<Type, List<Action<IMessage>>> routes = new Dictionary<Type, List<Action<IMessage>>>();


		private IList<ICommand> sentCommands = new List<ICommand>();

		public InProcessServiceBus(Type startingCommand, Dictionary<Type, Event> events)
		{
			_startingCommand = startingCommand;
			_events = events;
		}


		public async Task Send<T>(T command) where T : class, ICommand
		{
			if (command == null)
				throw new ArgumentNullException(nameof(command));

			sentCommands.Add(command);

			//Just for a fast test, do not this at home
			var handlerForCommand = new SagaToTest(this, new InMemorySagaRepository(new Serializer()));
			if (command.GetType() == _startingCommand)
			{
				await handlerForCommand.StartedBy((dynamic)command);
				return;
				//		return Task.CompletedTask;
			}

			var @event = _events[command.GetType()];
			if (@event != null)
				await handlerForCommand.Handle((dynamic)@event);
			//return Task.CompletedTask;
		}

		public Task RegisterHandler<T>(Action<T> handler) where T : IMessage
		{
			//if (!routes.TryGetValue(typeof(T), out var handlers))
			//{
			//	handlers = new List<Action<IMessage>>();
			//	routes.Add(typeof(T), handlers);
			//}
			//handlers.Add(x => handler((T)x));
			return Task.CompletedTask;
		}

		public IList<ICommand> SentCommands() => sentCommands;
	}
}
