using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostsFileManager
{
    public abstract class HostEntry
    {
        protected string formattingString { get; set; }

        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string IPAddress { get; set; }
        public string Domain { get; set; }
        public string Comment { get; set; }
        
        public int IPAddressColumnWidth = 0;
        public int DomainColumnWidth = 0;

        public override string ToString()
        {
            return String.Format(String.Format(formattingString, IPAddressColumnWidth, DomainColumnWidth, 2), IPAddress, Domain, Comment);
        }
    }
}
