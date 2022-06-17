using NettyServer;
using RPCServerLib.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib.Server
{
    public class NettyRpcServer : IRpcServer
    {
        private DotNettyServer Server;
        private int Port;
        public NettyRpcServer(int port)
        {
            this.Port = port;
            Server = new DotNettyServer();
        }

        public void Start()
        {
            Server.Listen(Port, (requestData) =>
            {
                var request = JsonConvert.DeserializeObject<RpcRequest>(requestData);
                var response = RpcConatiner.Invoke(request);
                return JsonConvert.SerializeObject(response);
            });
        }
        public void Close()
        {
            Server.Close();
        }

        public void RegisterService<IService, Service>() where Service : class, IService where IService : class
        {
            RpcConatiner.RegisterService<IService, Service>();
        }
    }
}
