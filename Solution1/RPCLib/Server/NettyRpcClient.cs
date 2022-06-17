using NettyServer;
using RPCServerLib.Proxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib.Server   
{
    public class NettyRpcClient : IRpcClient
    {
        private DotNettyClient Client;
        public NettyRpcClient(string ip, int port)
        {
            Client = new DotNettyClient(ip, port);
        }
        public void Start()
        {
            Client.Start();
        }
        public T Resolve<T>() where T : class
        {
            return InterfaceProxy.Resolve<T>(this);
        }
        public string Send(string message)
        {
            return Client.Send(message).Message;
        }
        public void Close()
        {
            Client.Close();
        }
    }
}
