using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudDancerCommon
{
    public class Server
    {
        public const Int32 DefaultPort = 31415;
        public const Int32 CurrentVersion = 1;
        public string Name { get; private set; }
        public Int32 Version { get; private set; }
        public string Address { get; private set; }
        public Int32 Port { get; private set; }
        public bool IsSelf { get; private set; }
        private Server(){}

        public static Server getSelf(AddressFamily family)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var self = new Server
            {
                Name = Environment.MachineName,
                Version = CurrentVersion,
                Address = host.AddressList.First(ip => ip.AddressFamily == family).ToString(),
                Port = DefaultPort,
                IsSelf = true
            };
            return self;
        }

        // ReSharper disable once InconsistentNaming
        public string toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return toJSON();
        }

        // ReSharper disable once InconsistentNaming
        public static Server fromJSON(string json)
        {
            return JsonConvert.DeserializeObject<Server>(json);
        }
    }
}
