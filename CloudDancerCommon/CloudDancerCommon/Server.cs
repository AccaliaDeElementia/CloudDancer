using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudDancerCommon
{
    public class Server
    {
        public const Int32 DefaultPort = 31415;
        public const Int32 CurrentVersion = 1;
        public string Name { get; private set; }
        public Int32 Version { get; private set; }
        public IPAddress Address { get; private set; }
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
                Address = host.AddressList.First(ip => ip.AddressFamily == family),
                Port = DefaultPort,
                IsSelf = true
            };
            return self;
        }

        private bool anouncing = false;
        public void BeginAnnounce(TimeSpan interval)
        {
            if (anouncing) return;
            anouncing = true;
            var task = new Thread(() =>
            {
                var udp = new UdpClient();
                var endpoint = new IPEndPoint(IPAddress.Broadcast, Port);
                var message = GetPayload();
                while (anouncing)
                {
                    udp.Send(message, message.Length, endpoint);
                    Thread.Sleep(interval);
                }
            });
            task.Start();
        }

        public void EndAnnounce()
        {
            anouncing = false;
        }
        private byte[] getBytes(int number)
        {
            var bytes = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private byte[] GetPayload()
        {
            var nullByte = new byte[] {0};
            IEnumerable<byte> payload = nullByte;
            payload = payload.Concat(Encoding.UTF8.GetBytes(Name));
            payload = payload.Concat(nullByte);
            payload = payload.Concat(getBytes(Version));
            payload = payload.Concat(nullByte); 
            payload = payload.Concat(Encoding.UTF8.GetBytes(Address.ToString()));
            payload = payload.Concat(nullByte);
            payload = payload.Concat(getBytes(Port));
            return payload.ToArray();
        }
    }
}
