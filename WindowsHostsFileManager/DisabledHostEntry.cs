using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostsFileManager
{
    public class DisabledHostEntry : HostEntry
    {
        public DisabledHostEntry()
        {
            formattingString = "#\t{{0,{0}}}\t\t{{1,{1}}}\t\t# {{{2}}}";
        }
    }
}
