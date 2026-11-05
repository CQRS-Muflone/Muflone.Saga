#nullable enable
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Muflone.Saga.Handlers;

/// <summary>
/// Opens the consumer span that Muflone's handler base classes open for every other message.
/// </summary>
/// <remarks>
/// The saga handlers implement the handler interfaces directly instead of deriving from
/// <c>CommandHandlerAsync</c> and <c>IntegrationEventHandlerAsync</c>, so without this nothing would
/// read the sender's trace context off the message: every saga step would start a trace of its own,
/// cut off from the request that caused it. The source names are Muflone's, so an application that
/// already listens to Muflone's handlers sees these spans with no extra registration.
/// </remarks>
internal static class SagaConsumerActivity
{
	private static readonly ActivitySource CommandSource =
		new(OpenTelemetryConstants.ActivitySourceNames.CommandHandler, OpenTelemetryConstants.Version);

	private static readonly ActivitySource IntegrationEventSource =
		new(OpenTelemetryConstants.ActivitySourceNames.IntegrationEventHandler, OpenTelemetryConstants.Version);

	public static Task TraceCommandAsync<TCommand>(TCommand command, Func<Task> deliver)
		where TCommand : ICommand
		=> TraceAsync(CommandSource, typeof(TCommand).Name, command, MessageHelpers.GetCorrelationId(command), deliver);

	public static Task TraceIntegrationEventAsync<TEvent>(TEvent @event, Func<Task> deliver)
		where TEvent : IEvent
		=> TraceAsync(IntegrationEventSource, typeof(TEvent).Name, @event, MessageHelpers.GetCorrelationId(@event), deliver);

	private static async Task TraceAsync(ActivitySource source, string messageType, IMessage message, Guid correlationId,
		Func<Task> deliver)
	{
		using var activity = OpenTelemetryMessageHelpers.TryExtractParentContext(message, out var parentContext)
			? source.StartActivity(messageType, ActivityKind.Consumer, parentContext)
			: source.StartActivity(messageType, ActivityKind.Consumer);

		activity?.SetTag(OpenTelemetryConstants.Tags.MessagingOperation, OpenTelemetryConstants.TagValues.OperationConsume);
		activity?.SetTag(OpenTelemetryConstants.Tags.MessagingMessageType, messageType);
		activity?.SetTag(OpenTelemetryConstants.Tags.MessagingMessageId, message.MessageId.ToString());
		activity?.SetTag(OpenTelemetryConstants.Tags.MessagingMessageCorrelationId, correlationId.ToString());

		try
		{
			await deliver();
			activity?.SetStatus(ActivityStatusCode.Ok);
		}
		catch (Exception ex)
		{
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			activity?.AddException(ex);
			throw;
		}
	}
}
