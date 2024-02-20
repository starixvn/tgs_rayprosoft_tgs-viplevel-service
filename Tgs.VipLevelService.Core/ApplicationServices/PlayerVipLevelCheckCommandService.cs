using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Level.Common.Protos;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelCheckCommandService(
	ILogger<PlayerVipLevelCheckCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelCheckCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelCheckCommand command, CancellationToken cancellationToken = default)
	{
		var message = new VipLevelsLevelMessage
		{
			PlayerCode = command.PlayerCode
		};

		await messageSender.PublishAsync(
			subject: VipLevelsLevelNatsConst.Check,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			VipLevelsLevelNatsConst.Check,
			message);
	}
}
