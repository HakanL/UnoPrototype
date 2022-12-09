using System;
using System.Collections.Generic;
using System.Text;
using DMXCore.DMXCore100.Contracts;

namespace DMXCore.DMXCore100
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from Message Service & Dependency Injection";
        }
    }
}
