using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyServer
{
    public class TransportMessage
    {
        public string Id { set; get; }

        public string Message { set; get; }

        public TransoprtType TransoprtType { set; get; }
    }
}
