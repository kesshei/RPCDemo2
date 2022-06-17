using RPCServerLib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kRpc
{
    public static class RpcServerFactory
    {
        private static ConcurrentDictionary<int, IRpcServer> _services { get; } = new ConcurrentDictionary<int, IRpcServer>();
        public static IRpcServer GetServer(int port)
        {
            if (_services.TryGetValue(port, out IRpcServer rpcServer))
            {
                return rpcServer;
            }
            else
            {
                rpcServer = new NettyRpcServer(port);
                _services.TryAdd(port, rpcServer);
                return rpcServer;
            }
        }
    }
}
