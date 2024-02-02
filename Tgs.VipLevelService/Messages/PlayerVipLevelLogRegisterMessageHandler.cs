using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

public class PlayerVipLevelLogRegisterMessageHandler(
	ILogger<PlayerVipLevelLogRegisterMessageHandler> logger,
	ICommandService<PlayerVipLevelLogCreateCommand> service) : IMessageHandler<PlayerVipLevelLogRegisterRequest>
{
	public ValueTask HandleAsync(string subject, PlayerVipLevelLogRegisterRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: data.BrandId,
			PlayerCode: data.PlayerCode,
			Type: PlayerVipLevelLogType.Register,
			BeforeLevel: 0,
			BeforeAutoState: Core.Protos.PlayerVipLevelAutoState.Enable,
			AfterLevel: data.AfterLevel,
			AfterAutoState: data.AfterAutoState,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
