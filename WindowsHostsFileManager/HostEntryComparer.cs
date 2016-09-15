using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostsFileManager
{
    public class HostEntryComparer : IComparer<HostEntry>
    {
        public int Compare(HostEntry x, HostEntry y)
        {
            if ((x.GetType() != y.GetType()) || (x is CommentsHostEntry || y is CommentsHostEntry))
            {
                return 0;
            }
            else
            {
                string[] entryPartsx = x.Domain.Split('.');
                string[] entryPartsy = y.Domain.Split('.');
                for (int ix = 0; ix < entryPartsx.Length; ix++)
                {
                    if (entryPartsy.Length == ix) return 1;
                    if (entryPartsy[ix].CompareTo(entryPartsx[ix]) != 0)
                    {
                        return entryPartsy[ix].CompareTo(entryPartsx[ix]);
                    }
                }
                return 0;
            }
        }
    }
}
