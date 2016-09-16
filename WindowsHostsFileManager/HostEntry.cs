using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsHostsFileManager
{
    public abstract class HostEntry
    {
        protected string FormattingString { get; set; }
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string IpAddress { get; set; }
        public string Domain { get; set; }
        public string Description { get; set; }
        
        public int IpAddressColumnWidth = 0;
        public int DomainColumnWidth = 0;

        public override string ToString()
        {
            return String.Format(String.Format(FormattingString, IpAddressColumnWidth, DomainColumnWidth, 2), IpAddress, Domain, Description);
        }
    }
}
