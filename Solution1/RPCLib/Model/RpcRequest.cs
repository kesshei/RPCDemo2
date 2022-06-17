using System;
using System.Collections.Generic;
using System.Text;

namespace RPCServerLib.Model    
{
    public class RpcRequest
    {
        public string NameSpace { set; get; }
        public string Method { set; get; }
        public List<string> Parameters { set; get; } = new List<string>();
        public List<string> ParameterTypes { set; get; } = new List<string>();
    }
}
