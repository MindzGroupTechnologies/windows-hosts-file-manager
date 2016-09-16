using System.Linq;
using System.Reflection;

namespace WindowsHostsFileManager
{
    public static class ObjectExtensions
    {
        public static void CopyFrom<TS, TD>(this TD destinationObject, TS sourceObject)
            where TS : HostEntry
            where TD : HostEntry
        {
            PropertyInfo[] srcFields = sourceObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            PropertyInfo[] destFields = destinationObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            foreach (var property in srcFields)
            {
                var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
                if (dest != null && dest.CanWrite)
                    dest.SetValue(destinationObject, property.GetValue(sourceObject, null), null);
            }
        }
    }
}
