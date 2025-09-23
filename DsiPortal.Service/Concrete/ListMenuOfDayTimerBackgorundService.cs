using DsiPortal.Service.IService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Service.Concrete
{
    public class ListMenuOfDayTimerBackgorundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ListMenuOfDayTimerBackgorundService> _logger;

        public ListMenuOfDayTimerBackgorundService(ILogger<ListMenuOfDayTimerBackgorundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var listMenuofDay = scope.ServiceProvider.GetRequiredService<IMenuofDay>();

                        var result = listMenuofDay.IListMenuofDay();

                        if (result.Item1 == null || result.Item2 == null || result.Item3 == null || result.Item4 == null)
                        {
                            _logger.LogWarning("Bugün için yemek listesi bulunamadı.");
                        }
                        else
                        {
                            _logger.LogInformation("Bugünkü menü: {Eat1}, {Eat2}, {Eat3}, {Eat4}",
                                result.Item1, result.Item2, result.Item3, result.Item4);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ListMenuOfDayTimerBackgorundService çalışırken hata oluştu.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Günde 1 kez kontrol et
            }
        }
    }
}
