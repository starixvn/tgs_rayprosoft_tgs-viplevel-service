using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PatchVipLevelCommandService : ICommandService<PatchVipLevelCommand>
{
	private readonly ITgsRepository _tgsRepository;

	public PatchVipLevelCommandService(ITgsRepository tgsRepository)
	{
		_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
	}

	public ValueTask ExecuteAsync(PatchVipLevelCommand command, CancellationToken cancellationToken = default)
		=> _tgsRepository.ModifyVipLevelAsync(
			command.Id,
			command.ModifyModel,
			command.Modifier,
			cancellationToken);
}
