using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

public class PlayerVipLevelLogSystemMessageHandler(
	ILogger<PlayerVipLevelLogSystemMessageHandler> logger,
	ICommandService<PlayerVipLevelLogCreateCommand> service) : IMessageHandler<PlayerVipLevelLogSystemMessage>
{
	public ValueTask HandleAsync(string subject, PlayerVipLevelLogSystemMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: data.BrandId,
			PlayerCode: data.PlayerCode,
			Type: PlayerVipLevelLogType.System,
			BeforeLevel: data.BeforeLevel ?? 0,
			BeforeAutoState: data.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE,
			AfterLevel: data.AfterLevel,
			AfterAutoState: data.AfterAutoState,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
