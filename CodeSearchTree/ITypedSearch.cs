using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
    public interface ITypedSearch
    {
        Node GetChild(params SearchNode[] sn);
    }
}
