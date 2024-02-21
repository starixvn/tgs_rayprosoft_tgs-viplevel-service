using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Messages;

internal class PlayerVipLevelUpgradeFormAutoMessageHandler(
	ILogger<PlayerVipLevelUpgradeFormAutoMessageHandler> logger,
	ICommandService<PlayerVipLevelUpgradeFormAutoCommand> service) : IMessageHandler<VipLevelFormRequest>
{
	public ValueTask HandleAsync(string subject, VipLevelFormRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({message})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new PlayerVipLevelUpgradeFormAutoCommand(
				Id: data.FormId),
			cancellationToken: cancellationToken);
	}
}
