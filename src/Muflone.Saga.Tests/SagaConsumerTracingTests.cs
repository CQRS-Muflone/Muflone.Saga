using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages;
using Muflone.Saga.Handlers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Muflone.Saga.Tests.SagaMessageHandlerTests;

namespace Muflone.Saga.Tests
{
	/// <summary>
	/// A saga step has to continue the trace of the message that caused it: without a consumer span read
	/// off the message, everything the saga sends afterwards starts a trace of its own.
	/// </summary>
	public sealed class SagaConsumerTracingTests : IDisposable
	{
		private const string SenderSourceName = "Muflone.Saga.Tests.Sender";
		private static readonly ActivitySource SenderSource = new(SenderSourceName);

		private readonly ConcurrentBag<Activity> _stopped = new();
		private readonly ActivityListener _listener;
		private readonly Guid _correlationId = Guid.NewGuid();

		public SagaConsumerTracingTests()
		{
			_listener = new ActivityListener
			{
				ShouldListenTo = source => source.Name == SenderSourceName
				                           || source.Name == OpenTelemetryConstants.ActivitySourceNames.CommandHandler
				                           || source.Name == OpenTelemetryConstants.ActivitySourceNames.IntegrationEventHandler,
				Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
				ActivityStopped = activity => _stopped.Add(activity)
			};
			ActivitySource.AddActivityListener(_listener);
		}

		public void Dispose() => _listener.Dispose();

		[Fact]
		public async Task SagaStartedByCommandHandler_ContinuesTheTraceOfTheCommand()
		{
			var starter = new ActivityCapturingStarter();
			var handler = new SagaStartedByCommandHandler<StartOrderProcess>(
				new ISagaStartedByAsync<StartOrderProcess>[] { starter }, NullLoggerFactory.Instance);
			var (command, sender) = SentWithinATrace(StartCommand());

			await handler.HandleAsync(command);

			var consumer = ConsumerSpanOf(command);
			Assert.Equal(OpenTelemetryConstants.ActivitySourceNames.CommandHandler, consumer.Source.Name);
			Assert.Equal(nameof(StartOrderProcess), consumer.DisplayName);
			Assert.Equal(sender.TraceId, consumer.TraceId);
			Assert.Equal(sender.SpanId, consumer.ParentSpanId);
			Assert.Equal(ActivityStatusCode.Ok, consumer.Status);
			// The saga runs inside the span, so what it sends becomes a child of it.
			Assert.Equal(consumer.SpanId, starter.Current?.SpanId);
		}

		[Fact]
		public async Task SagaIntegrationEventHandler_ContinuesTheTraceOfTheEvent()
		{
			var saga = new ActivityCapturingEventSaga();
			var handler = new SagaIntegrationEventHandler<OrderCompleted>(
				new ISagaEventHandlerAsync<OrderCompleted>[] { saga }, NullLoggerFactory.Instance);
			var (@event, sender) = SentWithinATrace(new OrderCompleted(new OrderId(Guid.NewGuid()), _correlationId));

			await handler.HandleAsync(@event);

			var consumer = ConsumerSpanOf(@event);
			Assert.Equal(OpenTelemetryConstants.ActivitySourceNames.IntegrationEventHandler, consumer.Source.Name);
			Assert.Equal(nameof(OrderCompleted), consumer.DisplayName);
			Assert.Equal(sender.TraceId, consumer.TraceId);
			Assert.Equal(sender.SpanId, consumer.ParentSpanId);
			Assert.Equal(consumer.SpanId, saga.Current?.SpanId);
		}

		[Fact]
		public async Task ScopedSagaStartedByCommandHandler_ContinuesTheTraceOfTheCommand()
		{
			var services = new ServiceCollection()
				.AddSingleton<ActivityCapturingStarter>()
				.AddScoped<ISagaStartedByAsync<StartOrderProcess>>(sp => sp.GetRequiredService<ActivityCapturingStarter>())
				.BuildServiceProvider();
			var handler = new ScopedSagaStartedByCommandHandler<StartOrderProcess>(
				services.GetRequiredService<IServiceScopeFactory>(), NullLoggerFactory.Instance);
			var (command, sender) = SentWithinATrace(StartCommand());

			await handler.HandleAsync(command);

			var consumer = ConsumerSpanOf(command);
			Assert.Equal(sender.SpanId, consumer.ParentSpanId);
			Assert.Equal(consumer.SpanId, services.GetRequiredService<ActivityCapturingStarter>().Current?.SpanId);
		}

		[Fact]
		public async Task ScopedSagaIntegrationEventHandler_ContinuesTheTraceOfTheEvent()
		{
			var services = new ServiceCollection()
				.AddSingleton<ActivityCapturingEventSaga>()
				.AddScoped<ISagaEventHandlerAsync<OrderCompleted>>(sp => sp.GetRequiredService<ActivityCapturingEventSaga>())
				.BuildServiceProvider();
			var handler = new ScopedSagaIntegrationEventHandler<OrderCompleted>(
				services.GetRequiredService<IServiceScopeFactory>(), NullLoggerFactory.Instance);
			var (@event, sender) = SentWithinATrace(new OrderCompleted(new OrderId(Guid.NewGuid()), _correlationId));

			await handler.HandleAsync(@event);

			var consumer = ConsumerSpanOf(@event);
			Assert.Equal(sender.SpanId, consumer.ParentSpanId);
			Assert.Equal(consumer.SpanId, services.GetRequiredService<ActivityCapturingEventSaga>().Current?.SpanId);
		}

		[Fact]
		public async Task SagaStartedByCommandHandler_WithoutATraceContext_StartsANewTrace()
		{
			// A command sent outside any trace - a scheduled job, a test - still gets its span.
			var command = StartCommand();
			var handler = new SagaStartedByCommandHandler<StartOrderProcess>(
				new ISagaStartedByAsync<StartOrderProcess>[] { new ActivityCapturingStarter() }, NullLoggerFactory.Instance);

			await handler.HandleAsync(command);

			var consumer = ConsumerSpanOf(command);
			Assert.Equal(default, consumer.ParentSpanId);
			Assert.Equal(ActivityKind.Consumer, consumer.Kind);
		}

		[Fact]
		public async Task SagaIntegrationEventHandler_WhenASagaThrows_MarksTheSpanAsFailed()
		{
			var @event = new OrderCompleted(new OrderId(Guid.NewGuid()), _correlationId);
			var handler = new SagaIntegrationEventHandler<OrderCompleted>(
				new ISagaEventHandlerAsync<OrderCompleted>[] { new ThrowingEventSaga() }, NullLoggerFactory.Instance);

			await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(@event));

			var consumer = ConsumerSpanOf(@event);
			Assert.Equal(ActivityStatusCode.Error, consumer.Status);
			Assert.Equal("saga failed", consumer.StatusDescription);
			Assert.Contains(consumer.Events, e => e.Name == "exception");
		}

		[Fact]
		public async Task SagaStartedByCommandHandler_WhenCancelledBeforeStarting_OpensNoSpan()
		{
			var command = StartCommand();
			var handler = new SagaStartedByCommandHandler<StartOrderProcess>(
				new ISagaStartedByAsync<StartOrderProcess>[] { new ActivityCapturingStarter() }, NullLoggerFactory.Instance);

			await Assert.ThrowsAsync<OperationCanceledException>(
				() => handler.HandleAsync(command, new CancellationToken(true)));

			Assert.DoesNotContain(_stopped, a => IsConsumerSpanOf(a, command));
		}

		/// <summary>Stamps the message with the trace context of a sender span, as Muflone's bus does.</summary>
		private static (TMessage Message, ActivityContext Sender) SentWithinATrace<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			using var sender = SenderSource.StartActivity("send", ActivityKind.Producer)
			                   ?? throw new InvalidOperationException("the test listener is not sampling the sender");
			OpenTelemetryMessageHelpers.InjectTraceContext(message);
			return (message, sender.Context);
		}

		private Activity ConsumerSpanOf(IMessage message) => _stopped.Single(a => IsConsumerSpanOf(a, message));

		// Filtering by message id keeps spans of tests running in parallel out of each other's way.
		private static bool IsConsumerSpanOf(Activity activity, IMessage message)
			=> activity.Kind == ActivityKind.Consumer
			   && (string?)activity.GetTagItem(OpenTelemetryConstants.Tags.MessagingMessageId) == message.MessageId.ToString();

		private StartOrderProcess StartCommand()
			=> new(new OrderId(Guid.NewGuid()), _correlationId, Guid.NewGuid());

		private sealed class ActivityCapturingStarter : ISagaStartedByAsync<StartOrderProcess>
		{
			public Activity? Current { get; private set; }

			public Task StartedByAsync(StartOrderProcess command)
			{
				Current = Activity.Current;
				return Task.CompletedTask;
			}

			public void Dispose()
			{
			}
		}

		private sealed class ActivityCapturingEventSaga : ISagaEventHandlerAsync<OrderCompleted>
		{
			public Activity? Current { get; private set; }

			public Task HandleAsync(OrderCompleted @event)
			{
				Current = Activity.Current;
				return Task.CompletedTask;
			}

			public void Dispose()
			{
			}
		}
	}
}
