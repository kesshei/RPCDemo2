using kRpc;
using RpcInterface;
using RpcModel;
using System;

namespace RpcServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RpcServerDemo by 蓝创精英团队";
            var server = RpcServerFactory.GetServer(999);
            server.RegisterService<IDemo, Demo>();
            server.Start();

            Console.WriteLine("服务启动");
            Console.ReadLine();
        }
    }
}
