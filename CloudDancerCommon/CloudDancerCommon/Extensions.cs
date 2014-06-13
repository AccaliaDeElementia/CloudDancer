using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDancerCommon
{
    static class Extensions
    {
        public static byte[] ToBytes(this int self)
        {
            var bytes = BitConverter.GetBytes(self);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public static int FromBytes(this int self, byte[] bytes)
        {
            var part = bytes.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(part);
            }
            return BitConverter.ToInt32(part, 0);
        }
    }
}
