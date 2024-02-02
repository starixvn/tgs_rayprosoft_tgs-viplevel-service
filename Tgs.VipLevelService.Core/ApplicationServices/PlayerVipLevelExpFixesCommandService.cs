using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Exp.Worker.Common;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpFixesCommandService(
	ILogger<PlayerVipLevelExpFixesCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelExpFixesCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpFixesCommand command, CancellationToken cancellationToken = default)
	{
		var message = new VipLevelsExpCreateMessage
		{
			PlayerCode = command.PlayerCode
		};

		await messageSender.PublishAsync(
			subject: VipLevelsExpWorkerNatsConst.Create,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.Manual,
			message);
	}
}
