using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelRepositories(this IServiceCollection services, Action<DbContextOptionsBuilder> dbConfigure)
		=> services
			.AddDbContext<TgsContext>(dbConfigure)
			.AddTransient<ITgsRepository, TgsRepository>();
}
