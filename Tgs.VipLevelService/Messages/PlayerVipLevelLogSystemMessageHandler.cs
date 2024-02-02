using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

public class PlayerVipLevelLogSystemMessageHandler(
	ILogger<PlayerVipLevelLogSystemMessageHandler> logger,
	ICommandService<PlayerVipLevelLogCreateCommand> service) : IMessageHandler<PlayerVipLevelLogSystemRequest>
{
	public ValueTask HandleAsync(string subject, PlayerVipLevelLogSystemRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({message})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: data.BrandId,
			PlayerCode: data.PlayerCode,
			Type: PlayerVipLevelLogType.System,
			BeforeLevel: data.BeforeLevel,
			BeforeAutoState: data.AutoState,
			AfterLevel: data.AfterLevel,
			AfterAutoState: data.AutoState,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
