using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.ApplicationServices;

internal class CreateVipLevelCommandService : IGetService<CreateVipLevelCommand, Guid>
{
	private readonly ITgsRepository _tgsRepository;

	public CreateVipLevelCommandService(ITgsRepository tgsRepository)
	{
		_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
	}

	public async ValueTask<Guid> GetAsync(CreateVipLevelCommand command, CancellationToken cancellationToken = default)
	{
		var newId = Guid.NewGuid();

		await _tgsRepository.CreateVipLevelAsync(
			newId,
			command.BrandId,
			command.CreateModel,
			command.Modifier,
			cancellationToken).ConfigureAwait(false);

		return newId;
	}
}
