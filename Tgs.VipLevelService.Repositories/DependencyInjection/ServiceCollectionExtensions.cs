using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.VipLevelService;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<ITgsRepository, TgsRepository>()
		.AddTransient<IVipLevelRepository, VipLevelRepository>()
		.Decorate<IVipLevelRepository>((innerRepository, sp) => new VipLevelCacheRepository(redisDatabaseResolver(sp), innerRepository, TimeSpan.FromMinutes(10)))
		.AddTransient<IPlayerRepository, PlayerRepository>()
		.Decorate<IPlayerRepository>((innerRepository, sp) => new PlayerCacheRepository(redisDatabaseResolver(sp), innerRepository, TimeSpan.FromDays(1)))
		.AddTransient<IPlayerVipLevelLogRepository, PlayerVipLevelLogRepository>()
		.AddTransient<IPlayerVipLevelRepository, PlayerVipLevelRepository>()
		.Decorate<IPlayerVipLevelRepository>((innerRepository, sp) => new PlayerVipLevelCacheRepository(redisDatabaseResolver(sp), innerRepository, TimeSpan.FromMinutes(10)));
}
