﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace Muflone.Saga.Tests.Persistence
{
	public class InProcessServiceBus : IServiceBus
	{
		private readonly Type _startingCommand;
		private readonly Dictionary<Type, Event> _events;
		
		private IList<ICommand> sentCommands = new List<ICommand>();

		public InProcessServiceBus(Type startingCommand, Dictionary<Type, Event> events)
		{
			_startingCommand = startingCommand;
			_events = events;
		}


		public async Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand
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
			}

			var @event = _events[command.GetType()];
			if (@event != null)
				await handlerForCommand.Handle((dynamic)@event);
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
