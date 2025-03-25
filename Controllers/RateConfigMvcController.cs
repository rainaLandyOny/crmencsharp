using Microsoft.AspNetCore.Mvc;
using crmcsharp.Services;
using crmcsharp.Models.entity;
using crmcsharp.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace crmcsharp.Controllers
{
    public class RateConfigMvcController : Controller
    {
        private readonly RateConfigService _rateConfigService;
        private readonly ILogger<RateConfigMvcController> _logger;

        public RateConfigMvcController(RateConfigService rateConfigService, ILogger<RateConfigMvcController> logger)
        {
            _rateConfigService = rateConfigService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            _logger.LogInformation("Loading Index page with filters: {start} - {end}", startDate, endDate);

            var viewModel = new RateConfigViewModel
            {
                RateConfigs = await _rateConfigService.GetAllRateConfigsAsync(startDate, endDate),
                FilterStartDate = startDate,
                FilterEndDate = endDate
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new RateConfig { CreatedAt = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RateConfig rateConfig)
        {
            _logger.LogInformation("Rate Config: {Rate} - {CreatedAt}", rateConfig.Rate, rateConfig.CreatedAt);
            var result = await _rateConfigService.CreateRateConfigAsync(rateConfig);

            if (result == null)
            {
                TempData["Error"] = "Échec de la mise à jour de la dépense.";
            }
            else
            {
                TempData["Message"] = "Dépense mise à jour avec succès.";
            }
            return RedirectToAction("Index");
        }
    }
}