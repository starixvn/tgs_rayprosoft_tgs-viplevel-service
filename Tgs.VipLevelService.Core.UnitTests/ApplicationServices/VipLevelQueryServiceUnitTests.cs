using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelQueryServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var sut = new VipLevelQueryService(fakeVipLevelRepository);

		var query = new VipLevelQuery(Guid.NewGuid());

		_ = await sut.QueryAsync(query).ToArrayAsync();

		_ = fakeVipLevelRepository
			.Received(1)
			.QueryAsync(
			brandId: Arg.Is(query.BrandId),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
