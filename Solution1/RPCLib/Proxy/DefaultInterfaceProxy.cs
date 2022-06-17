using RPCServerLib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RPCServerLib.Model;

namespace RPCServerLib.Proxy    
{
    public interface InvocationHandler
    {
        object InvokeMember(object sender, int methodId, string name, params object[] args);
    }

    public class DefaultInvocationHandler<T> : InvocationHandler
    {
        readonly IRpcClient RpcClient;
        public DefaultInvocationHandler(IRpcClient rpcClient)
        {
            RpcClient = rpcClient;
        }
        public object InvokeMember(object sender, int methodId, string name, params object[] args)
        {
            var met = (MethodInfo)typeof(T).Module.ResolveMethod(methodId);
            var request = new RpcRequest();
            string[] names = name.Split('+');
            request.NameSpace = names[0];
            request.Method = names[1];
            if (args != null)
            {
                foreach (var arg in args)
                {
                    request.Parameters.Add(JsonConvert.SerializeObject(arg));
                }
                request.ParameterTypes.AddRange(met.GetParameters().Select(p => p.ParameterType.FullName));
            }
            var response = JsonConvert.DeserializeObject<RpcResponse>(RpcClient.Send(JsonConvert.SerializeObject(request)));// RpcConatiner.Invoke(request);
            if (response != null && response.Code == 0)
            {
                if (met.ReturnType == typeof(void))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject(response.Response, met.ReturnType);
            }
            else
            {
                throw new Exception(response.Message);
            }
        }
    }
}
