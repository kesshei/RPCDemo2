using RpcInterface;
using RPCServerLib.Attributes;
using System;
using System.Collections.Generic;

namespace RpcModel
{
    /// <summary>
    /// 服务端的接口逻辑实现
    /// </summary>
    [RpcService]
    public class Demo : IDemo
    {
        private List<string> _Test = new List<string>();
        public List<string> Test { get { return _Test; } set { _Test = value; } }

        public void Say()
        {
            Console.WriteLine("123456");
        }

        public string Say(string msg)
        {
            return msg;
        }

        public int Say(string a, int b, List<string> c, Kind kind)
        {
            return b;
        }

        public Kind Say(int b, List<string> c, Kind kind)
        {
            return kind;
        }
    }
}
