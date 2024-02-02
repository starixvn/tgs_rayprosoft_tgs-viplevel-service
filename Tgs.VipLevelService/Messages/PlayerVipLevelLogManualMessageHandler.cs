using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

public class PlayerVipLevelLogManualMessageHandler(
	ILogger<PlayerVipLevelLogManualMessageHandler> logger,
	ICommandService<PlayerVipLevelLogCreateCommand> service) : IMessageHandler<PlayerVipLevelLogManualRequest>
{
	public ValueTask HandleAsync(string subject, PlayerVipLevelLogManualRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: data.BrandId,
			PlayerCode: data.PlayerCode,
			Type: PlayerVipLevelLogType.Manual,
			BeforeLevel: data.BeforeLevel,
			BeforeAutoState: data.BeforeAutoState,
			AfterLevel: data.AfterLevel,
			AfterAutoState: data.AfterAutoState,
			CreatedUserId: data.CreatedUserId,
			CreatedUsername: data.CreatedUsername,
			Remark: data.Remark);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
