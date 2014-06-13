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
    public static class Gdm
    {
        public const string GdmGreeting = "GDM_CloudDancer";
        public const int GdmPort = 31416;
        private static byte[] GetPayload(Server self)
        {
            var message = Encoding.UTF8.GetBytes(GdmGreeting + self.toJSON());
            var size = (message.Length + 4).ToBytes();
            return size.Concat(message).ToArray();
        }
        private static bool _anouncing = false;
        public static void BeginAnnounce(TimeSpan interval, Server self=null)
        {
            if (_anouncing) return;
            if (self == null)
            {
                self = Server.getSelf(AddressFamily.InterNetwork);
            }
            _anouncing = true;
            var task = new Thread(() =>
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
    ProtocolType.Udp);

                IPAddress broadcast = IPAddress.Broadcast;
                s.EnableBroadcast = true;

                IPEndPoint ep = new IPEndPoint(broadcast, GdmPort);
                var message = GetPayload(self);
                while (_anouncing)
                {
                    s.SendTo(message, ep); 
                    Thread.Sleep(interval);
                }
            });
            task.Start();
        }

        public static void EndAnnounce()
        {
            _anouncing = false;
        }

        public static IEnumerable<Server> Listen(ulong limit)
        {
            var udp = new UdpClient(GdmPort);
            var endpoint = new IPEndPoint(IPAddress.Any, GdmPort);
            ulong count=0;
            while (limit < 1 || count < limit)
            {
                var datagram = udp.Receive(ref endpoint);
                var length = 0.FromBytes(datagram);
                if (length != datagram.Length) continue;
                var data = Encoding.UTF8.GetString(datagram.Skip(4).ToArray());
                if (!data.StartsWith(GdmGreeting)) continue;
                data = data.Substring(GdmGreeting.Length);
                yield return Server.fromJSON(data);
                if (limit >=1 ) count ++;
            }
        }
    }
}
