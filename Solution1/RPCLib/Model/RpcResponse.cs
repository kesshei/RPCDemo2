using System;
using System.Collections.Generic;
using System.Text;

namespace RPCServerLib.Model    
{
    public class RpcResponse
    {
        public int Code { set; get; }

        public string Message { set; get; }

        public string Response { set; get; }

        public RpcRequest Request { get; set; }
        public void SetState(int code, string msg)
        {
            Code = code;
            Message = msg;
        }
        public void SetState(string msg)
        {
            Message = msg;
        }
    }
}
