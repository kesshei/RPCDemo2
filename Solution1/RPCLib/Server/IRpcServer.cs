using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib.Server   
{
    public interface IRpcServer
    {
        void Start();
        void RegisterService<IService, Service>() where Service : class, IService where IService : class;
        void Close();
    }
}
