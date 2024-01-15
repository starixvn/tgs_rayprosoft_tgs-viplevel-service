using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.Models;

public record PlayerVipLevelInfo(
   string PlayerCode,
   Guid VipLevelId,
   int VipLevel = 0,
   PlayerVipLevelAutoState AutoState = PlayerVipLevelAutoState.ENABLE);
