using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead
{
    public interface ISheepOutputter
    {
        void Output(string message, SHPlayer pl = null);
    }
    public interface ISheepInputter
    {
        string Input();
    }
}
