using RPCServerLib.Attributes;
using RPCServerLib.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib
{
    public static class RpcConatiner
    {
        private static ConcurrentDictionary<string, object> ServiceContainer = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, Type> ServiceTypeContainer = new ConcurrentDictionary<string, Type>();
        static RpcConatiner()
        {
            Initialize();
        }

        private static void Initialize()
        {
            try
            {
                foreach (var assembly in  AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        RpcServiceAttribute attribute = type.GetCustomAttribute<RpcServiceAttribute>();
                        if (attribute != null)
                        {
                            var obj = Activator.CreateInstance(type);
                            foreach (var iInterface in type.GetInterfaces())
                            {
                                ServiceContainer.AddOrUpdate(iInterface.FullName, obj, (k, v) => obj);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public static void RegisterService<IService, Service>() where Service : class, IService where IService : class
        {
            ServiceTypeContainer.AddOrUpdate(typeof(IService).FullName, typeof(Service), (k, v) => typeof(Service));
        }
        public static RpcResponse Invoke(RpcRequest request)
        {
            var rpcResponse = new RpcResponse() { Code = 1, Message = "未知错误", Request = request };
            rpcResponse.SetState($"{request.NameSpace} 命名空间未存在!");
            if (ServiceContainer.TryGetValue(request.NameSpace, out object obj))
            {
                rpcResponse.SetState($"{request.Method} 方法未存在!");
                try
                {
                    var Types = request.GetTypes();
                    var method = obj.GetType().GetMethod(request.Method, Types);
                    if (method != null)
                    {
                        rpcResponse.SetState($"执行结果异常!");
                        rpcResponse.Response = JsonConvert.SerializeObject(method.Invoke(obj, request.GetParameters(Types)));
                        rpcResponse.SetState(0, "成功");
                    }
                }
                catch (Exception ex)
                {
                    rpcResponse.SetState(-1, ex.Message);
                }
            }
            return rpcResponse;
        }
        private static Type[] GetTypes(this RpcRequest request)
        {
            var types = new List<Type>();
            foreach (var paramType in request.ParameterTypes)
            {
                Type type = GetType(paramType);
                if (type == null)
                {
                    return Type.EmptyTypes;
                }
                types.Add(type);
            }
            if (types.Any())
            {
                return types.ToArray();
            }
            return Type.EmptyTypes;
        }
        private static object[] GetParameters(this RpcRequest request, Type[] Types)
        {
            var list = new List<object>();
            if (Types == null || !Types.Any())
            {
                var types = request.GetTypes();
                if (types == null || !types.Any())
                {
                    return null;
                }
            }

            for (int i = 0; i < Types.Length; i++)
            {
                list.Add(JsonConvert.DeserializeObject(request.Parameters[i], Types[i]));
            }
            return list.ToArray();
        }
        private static Type GetType(string typeFullName)
        {
            if (ServiceTypeContainer.TryGetValue(typeFullName, out Type Type))
            {
                return Type;
            }
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = ass.GetType(typeFullName);
                if (type != null)
                {
                    ServiceTypeContainer.TryAdd(type.FullName, type);
                    return type;
                }
            }
            return null;
        }
    }
}
