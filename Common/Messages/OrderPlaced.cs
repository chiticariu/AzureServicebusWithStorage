﻿using NServiceBus;

namespace Messages
{
    public class OrderPlaced :
        IEvent
    {
        public string OrderId { get; set; }

        public string PayloadLocation { get; set; }
    }
}