using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PatchVipLevelCommandService(ITgsRepository tgsRepository) : ICommandService<PatchVipLevelCommand>
{
	public ValueTask ExecuteAsync(PatchVipLevelCommand command, CancellationToken cancellationToken = default)
		=> tgsRepository.ModifyVipLevelAsync(
			command.Id,
			command.ModifyModel,
			command.Modifier,
			cancellationToken);
}
