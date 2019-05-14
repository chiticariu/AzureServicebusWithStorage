using Messages;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;
using Core;

namespace Shipping.Webjob.Handlers
{
    public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        private readonly ILog _log = LogManager.GetLogger<OrderPlacedHandler>();
        private readonly StorageRepository _storageRepository;

        public OrderPlacedHandler(StorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            _log.Info($"Billing has received OrderPlaced, OrderId = {message.OrderId}");

            var storageMessage = await _storageRepository
                .RestoreMessageFromStorage<string>(message.PayloadLocation);
            _log.Info($"The OrderPlaced storage message = {storageMessage}");
        }
    }
}