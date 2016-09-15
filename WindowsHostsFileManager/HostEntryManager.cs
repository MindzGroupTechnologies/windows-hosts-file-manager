using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsHostsFileManager
{
    public class HostEntryManager
    {
        public List<HostEntry> hostEntries { get; private set; }
        string hostsFilePath = null;

        public HostEntryManager()
        {
            hostsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
        }

        public void LoadHostsFile()
        {
            List<string> hostEntryStrings = File.ReadAllLines(hostsFilePath).ToList();
            hostEntries = new List<HostEntry>();
            int lineNumber = 1;
            foreach (var item in hostEntryStrings)
            {
                HostEntry entry = buildHostEntry(item);
                entry.LineNumber = lineNumber++;
                hostEntries.Add(entry);
            }
            UpdateColumnWidth();
        }

        public void Save()
        {
            hostEntries = hostEntries.OrderBy(i => i, new HostEntryComparer()).ToList();
            List<String> output = hostEntries.Select<HostEntry, String>(en => en.ToString()).ToList();
            string outputText = String.Join(Environment.NewLine, output.ToArray());
            File.WriteAllLines(hostsFilePath, output);
            LoadHostsFile();
        }

        private HostEntry buildHostEntry(string lineText)
        {
            HostEntry entry = null;
            Regex rx = new Regex(@"^(?:(?=\s*\d+?\..*)(?<Enabled>\s*(?<IP>\d+?\.\d+?\.\d+?\.\d+?)\s*(?<Domain>[a-zA-Z\.\-0-9]*?)(?:(?:\s*)|(?:\s*\#\s*(?<Comment>.*?))))|(?:(?=\s*?\#*?\s*\d+?\..*)(?<Disabled>\s*\#*?\s*(?<DIP>\d+?\.\d+?\.\d+?\.\d+?)\s*(?<DDomain>[a-zA-Z\.\-0-9]*?)(?:(?:\s*)|(?:\s*\#\s*(?<DComment>.*?))))|(?<Comments>.*)))$");
            string[] groupNames = rx.GetGroupNames();
            var matches = rx.Matches(lineText);
            int matchCount = matches.Count;
            if (matchCount > 0)
            {
                foreach (Match match in matches)
                {
                    Debug.WriteLine("match: " + match.Value + " have " + match.Groups.Count + " groups");
                    var selectedGroups = groupNames.Where(g => ((g == "Enabled" || g == "Disabled" || g == "Comments" || g == "DIP" || g == "DDomain" || g == "DComment" || g == "IP" || g == "Domain" || g == "Comment") && match.Groups[g].Captures.Count > 0));

                    if (selectedGroups.Contains("Enabled"))
                    {
                        entry = new EnabledHostEntry();
                    }
                    else
                        if (selectedGroups.Contains("Disabled"))
                        {
                            entry = new DisabledHostEntry();
                        }
                        else
                        {
                            entry = new CommentsHostEntry();
                        }
                    entry.LineText = lineText;
                    foreach (string groupName in selectedGroups)
                    {
                        Group group = match.Groups[groupName];
                        Debug.WriteLine("\tgroup: " + groupName + " have " + group.Captures.Count + " captures");
                        foreach (Capture capture in group.Captures)
                        {
                            switch (groupName)
                            {
                                case "Comments":
                                    entry.Comment = group.Value;
                                    break;
                                case "IP":
                                case "DIP":
                                    entry.IPAddress = group.Value;
                                    break;
                                case "Domain":
                                case "DDomain":
                                    entry.Domain = group.Value;
                                    break;
                                case "Comment":
                                case "DComment":
                                    entry.Comment = group.Value;
                                    break;
                            }
                            Debug.WriteLine("\t\tcapture: " + capture.Value);
                        }
                    }
                }
            }
            return entry;
        }

        private void UpdateColumnWidth()
        {
            int IPAddressColumnWidth = hostEntries.Max(en => en.IPAddress != null ? en.IPAddress.Length : 0);
            int DomainColumnWidth = hostEntries.Max(en => en.Domain != null ? en.Domain.Length : 0);
            hostEntries.ForEach(en => { en.DomainColumnWidth = DomainColumnWidth; en.IPAddressColumnWidth = IPAddressColumnWidth; });
        }
    }
}
