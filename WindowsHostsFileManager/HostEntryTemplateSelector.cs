using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindowsHostsFileManager
{
    public class HostEntryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentsTemplate { get; set; }
        public DataTemplate EnabledTemplate { get; set; }
        public DataTemplate DisabledTemplate { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {

            if (item is EnabledHostEntry)
            {
                return EnabledTemplate;
            }
            else if (item is DisabledHostEntry)
            {
                return DisabledTemplate;
            }
            else {
                return CommentsTemplate;
            }
        }
    }
}
