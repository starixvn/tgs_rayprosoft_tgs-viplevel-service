using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

internal class VipLevelSnapshotMessageHandler(
	ILogger<VipLevelSnapshotMessageHandler> logger,
	ICommandService<VipLevelSnapshotCreateCommand> service) : IMessageHandler<VipLevelSnapshotRequest>
{
	public ValueTask HandleAsync(string subject, VipLevelSnapshotRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({message})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		var command = new VipLevelSnapshotCreateCommand(
			BrandId: data.BrandId,
			EffectiveOn: data.EffectiveOn.ToDateTimeOffset(),
			SnapshotUserId: data.SnapshotUserId.ToGuid(),
			SnapshotUsername: data.SnapshotUsername);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
