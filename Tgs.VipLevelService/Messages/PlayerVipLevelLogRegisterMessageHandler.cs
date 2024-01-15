using System.Text;

using System.Text.Json;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.MessageModels;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

public class PlayerVipLevelLogRegisterMessageHandler(
	ILogger<PlayerVipLevelLogRegisterMessageHandler> logger,
	ICommandService<PlayerVipLevelLogCreateCommand> service) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<PlayerVipLevelLogRegisterMessageModel>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({message})",
			DateTime.Now.ToString("o"),
			subject,
			message);

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: message.BrandId,
			PlayerCode: message.PlayerCode,
			Type: PlayerVipLevelLogType.Register,
			BeforeLevel: 0,
			BeforeAutoState: PlayerVipLevelAutoState.Enable,
			AfterLevel: message.AfterLevel,
			AfterAutoState: message.AfterAutoState,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
