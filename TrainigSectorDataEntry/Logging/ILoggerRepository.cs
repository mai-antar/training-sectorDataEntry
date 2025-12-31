using System;
using Microsoft.AspNetCore.Mvc;
namespace TrainigSectorDataEntry.Logging
{
    public interface ILoggerRepository
    {
        void LogError(Exception ex,string controllerName, string actionName);
    
    }
}



