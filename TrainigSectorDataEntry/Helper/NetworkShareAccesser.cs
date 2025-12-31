using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrainigSectorDataEntry.Helper
{
    public class NetworkShareAccesser : IDisposable
    {
        string _networkName;

        public NetworkShareAccesser(string networkName, string userName, string password)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = 2,
                ResourceType = 1,
                DisplayType = 3,
                RemoteName = networkName
            };

            var result = WNetAddConnection2(
                netResource,
                password,
                userName,
                0);

            if (result != 0)
            {
                throw new IOException("Error connecting to remote share", result);
            }
        }

        ~NetworkShareAccesser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public int Scope;
            public int ResourceType;
            public int DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }
    }

}
