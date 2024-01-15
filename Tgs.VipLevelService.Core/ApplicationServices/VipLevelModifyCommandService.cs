using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;
internal class VipLevelModifyCommandService(IVipLevelRepository repository) : ICommandService<VipLevelModifyCommand>
{
	public async ValueTask ExecuteAsync(VipLevelModifyCommand command, CancellationToken cancellationToken = default)
	{
		var vipLevelInfo = await repository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();

		await repository.UpdateAsync(
			id: vipLevelInfo.Id,
			brandId: vipLevelInfo.BrandId,
			modifyModel: command.ModifyModel,
			modifier: command.Modifier,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}

