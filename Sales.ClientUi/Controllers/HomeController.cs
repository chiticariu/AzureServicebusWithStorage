using Core;
using Messages;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using NServiceBus.Logging;
using Sales.ClientUi.Models;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Sales.ClientUi.Controllers
{
    public class HomeController : Controller
    {
        private ILog _log = LogManager.GetLogger<HomeController>();
        private readonly IMessageSession _messageSession;
        private readonly StorageRepository _storageRepository;

        public HomeController(IMessageSession messageSession,
            StorageRepository storageRepository)
        {
            _messageSession = messageSession;
            _storageRepository = storageRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var orderId = Guid.NewGuid().ToString().Substring(0, 8);

            var payloadLocation = await _storageRepository
                .SaveMessageToStorage("very big message from storage");

            //Create the order then publish that the order was placed
            var orderPlaced = new OrderPlaced
            {
                OrderId = orderId,
                PayloadLocation = payloadLocation
            };

            _log.Info($"Publishing OrderPlaced, OrderId = {orderId}");

            await _messageSession.Publish(orderPlaced)
                .ConfigureAwait(false);

            dynamic model = new ExpandoObject();
            model.OrderId = orderId;

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
