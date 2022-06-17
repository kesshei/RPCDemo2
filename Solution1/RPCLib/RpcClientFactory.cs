using RPCServerLib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib
{
    public static class RpcClientFactory
    {
        private static ConcurrentDictionary<string, IRpcClient> _services { get; } = new ConcurrentDictionary<string, IRpcClient>();
        public static IRpcClient GetClient(string serverIp, int port)
        {
            var key = $"{serverIp}_{port}";
            if (_services.TryGetValue(key, out IRpcClient rpcClient))
            {
                return rpcClient;
            }
            else
            {
                rpcClient = new NettyRpcClient(serverIp, port);
                _services.TryAdd(key, rpcClient);
                return rpcClient;
            }
        }
    }
}
