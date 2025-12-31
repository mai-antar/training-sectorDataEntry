using Microsoft.Extensions.Logging;
using System;
using TrainigSectorDataEntry.Logging;

namespace TrainigSectorDataEntry.Logging
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly ILogger<LoggerRepository> _logger;

        public LoggerRepository(ILogger<LoggerRepository> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string controllerName, string actionName)
        {
            var message = ex.InnerException?.InnerException?.Message
                       ?? ex.InnerException?.Message
                       ?? ex.Message;

            _logger.LogError("[{Controller}.{Action}] {Message}", controllerName, actionName, message);
        }


        
    }
}
