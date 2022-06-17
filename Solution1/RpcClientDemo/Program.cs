using RpcInterface;
using RPCServerLib;
using System;
using System.Collections.Generic;

namespace RpcClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RpcClientDemo by 蓝创精英团队";
            var client = RpcClientFactory.GetClient("127.0.0.1", 999);
            client.Start();
            Console.WriteLine("客户端开始连接!");

            var demo = client.Resolve<IDemo>();
            demo.Say();
            Console.WriteLine(demo.Say("123"));
            Console.WriteLine(demo.Say("demo", 6, new List<string>() { "6" }, Kind.b));
            Console.WriteLine("不错，完成了任务!");

            while (Console.ReadLine().StartsWith("Exit", StringComparison.InvariantCultureIgnoreCase))
            {
                break;
            }
            Console.ReadLine();
        }
    }
}
