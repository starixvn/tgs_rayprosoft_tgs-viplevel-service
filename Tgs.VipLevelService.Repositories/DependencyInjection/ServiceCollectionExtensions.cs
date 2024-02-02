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
		.Decorate<IVipLevelRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<VipLevelCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(5)))
		.Decorate<IVipLevelRepository, VipLevelMessageRepository>()
		.AddTransient<IPlayerRepository, PlayerRepository>()
		.Decorate<IPlayerRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromDays(1)))
		.AddTransient<IPlayerVipLevelLogRepository, PlayerVipLevelLogRepository>()
		.AddTransient<IPlayerVipLevelRepository, PlayerVipLevelRepository>()
		.Decorate<IPlayerVipLevelRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<PlayerVipLevelCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(10)))
		.AddTransient<IPlayerVipLevelExpRepository, PlayerVipLevelExpRepository>()
		.AddTransient<IVipLevelSnapshotRepository, VipLevelSnapshotRepository>();
}
