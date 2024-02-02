using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelSystemLogPublishCommandService(
	ILogger<PlayerVipLevelSystemLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelSystemLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelSystemLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var request = new PlayerVipLevelLogSystemRequest
		{
			BrandId = command.BrandId,
			PlayerCode = command.PlayerCode,
			BeforeLevel = command.BeforeLevel.GetValueOrDefault(0),
			AfterLevel = command.AfterLevel,
			AutoState = command.AutoState
		};

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.System,
			data: request.ToByteArray(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.System,
			request);
	}
}
