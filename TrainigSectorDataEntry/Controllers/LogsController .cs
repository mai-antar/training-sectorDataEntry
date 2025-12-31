using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace TrainigSectorDataEntry.Controllers
{
    public class LogsController : Controller
    {
        public IActionResult Index()
        {
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", $"errors-{DateTime.Now:yyyyMMdd}.txt");

            List<string> errorLines = new();

            if (System.IO.File.Exists(logPath))
            {
                var allLines = System.IO.File.ReadAllLines(logPath);

                var regex = new Regex(@"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} ERR\]");

                errorLines = allLines
                    .Where(line => regex.IsMatch(line))
                    .ToList();
            }
            else
            {
                errorLines.Add("⚠️ لا يوجد أخطاء مسجلة اليوم.");
            }

            return View(errorLines);
        }
    }
}
