using System;
using System.Collections.Generic;
using System.Text;

namespace RPCServerLib.Attributes   
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class RpcServiceAttribute : Attribute
    {
    }
}
