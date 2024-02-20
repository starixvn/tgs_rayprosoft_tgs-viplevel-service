using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelRegisterLogPublishCommandService(
	ILogger<PlayerVipLevelRegisterLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelRegisterLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelRegisterLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var request = new PlayerVipLevelLogRegisterRequest
		{
			BrandId = command.BrandId,
			PlayerCode = command.PlayerCode,
			AfterLevel = command.AfterLevel,
			AfterAutoState = command.AfterAutoState
		};

		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.PlayerVipLevelLogRegister,
			data: request.ToByteArray(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			VipLevelServiceNatsConsts.PlayerVipLevelLogRegister,
			request);
	}
}
