using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace WindowsHostsFileManager
{
    public class HostEntryManager
    {
        private readonly string _hostsFilePath;

        public List<HostEntry> HostEntries { get; private set; }

        private bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

        public HostEntryManager()
        {
            _hostsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc",
                "hosts");
            if (IsInDesignMode == true)
            {
                LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
            HostEntries = new List<HostEntry>();
            HostEntries.Add(new CommentsHostEntry() {LineNumber = 1, LineText = "# Comment"});
            HostEntries.Add(new EnabledHostEntry()
            {
                LineNumber = 1,
                LineText = "# Comment",
                IpAddress = "0.0.0.0",
                Domain = "localhost",
                Description = "Description"
            });
            HostEntries.Add(new DisabledHostEntry()
            {
                LineNumber = 1,
                LineText = "# Comment",
                IpAddress = "0.0.0.0",
                Domain = "localhost",
                Description = "Description"
            });
        }

        public void LoadHostsFile()
        {
            var hostEntryStrings = File.ReadAllLines(_hostsFilePath).ToList();
            HostEntries = new List<HostEntry>();
            var lineNumber = 1;
            foreach (var item in hostEntryStrings)
            {
                var entry = BuildHostEntry(item);
                entry.LineNumber = lineNumber++;
                HostEntries.Add(entry);
            }
            UpdateColumnWidth();
        }

        public void Save()
        {
            //HostEntries = HostEntries.OrderBy(i => i, new HostEntryComparer()).ToList();
            var output = HostEntries.Select(en => en.ToString()).ToList();
            File.WriteAllLines(_hostsFilePath, output);
            LoadHostsFile();
        }

        private HostEntry BuildHostEntry(string lineText)
        {
            HostEntry entry = null;
            var rx =
                new Regex(
                    @"^(?:(?=\s*\d+?\..*)(?<Enabled>\s*(?<IP>\d+?\.\d+?\.\d+?\.\d+?)\s*(?<Domain>[a-zA-Z\.\-0-9]*?)(?:(?:\s*)|(?:\s*\#\s*(?<Comment>.*?))))|(?:(?=\s*?\#*?\s*\d+?\..*)(?<Disabled>\s*\#*?\s*(?<DIP>\d+?\.\d+?\.\d+?\.\d+?)\s*(?<DDomain>[a-zA-Z\.\-0-9]*?)(?:(?:\s*)|(?:\s*\#\s*(?<DComment>.*?))))|(?<Comments>.*)))$");
            var groupNames = rx.GetGroupNames();
            var matches = rx.Matches(lineText);
            var matchCount = matches.Count;
            if (matchCount > 0)
            {
                foreach (Match match in matches)
                {
                    Debug.WriteLine("match: " + match.Value + " have " + match.Groups.Count + " groups");
                    var currentMatch = match;
                    var selectedGroups =
                        groupNames.Where(
                            g =>
                                (g == "Enabled" || g == "Disabled" || g == "Comments" || g == "DIP" ||
                                 g == "DDomain" || g == "DComment" || g == "IP" || g == "Domain" || g == "Comment") &&
                                currentMatch.Groups[g].Captures.Count > 0).ToList();

                    if (selectedGroups.Contains("Enabled"))
                    {
                        entry = new EnabledHostEntry();
                    }
                    else if (selectedGroups.Contains("Disabled"))
                    {
                        entry = new DisabledHostEntry();
                    }
                    else
                    {
                        entry = new CommentsHostEntry();
                    }

                    entry.LineText = lineText;

                    foreach (var groupName in selectedGroups)
                    {
                        var group = match.Groups[groupName];
                        Debug.WriteLine("\tgroup: {0} have {1} captures", groupName, @group.Captures.Count);
                        if (groupName == "Comments")
                        {
                            entry.Description = @group.Value;
                        }
                        else if (groupName == "IP" || groupName == "DIP")
                        {
                            entry.IpAddress = @group.Value;
                        }
                        else if (groupName == "Domain" || groupName == "DDomain")
                        {
                            entry.Domain = @group.Value;
                        }
                        else if (groupName == "Comment" || groupName == "DComment")
                        {
                            entry.Description = @group.Value;
                        }
                        Debug.WriteLine("\t\tcapture: " + @group.Captures[0].Value);
                    }
                }
            }
            return entry;
        }

        private void UpdateColumnWidth()
        {
            int ipAddressColumnWidth = HostEntries.Max(en => en.IpAddress != null ? en.IpAddress.Length : 0);
            int domainColumnWidth = HostEntries.Max(en => en.Domain != null ? en.Domain.Length : 0);
            HostEntries.ForEach(en =>
            {
                en.DomainColumnWidth = domainColumnWidth;
                en.IpAddressColumnWidth = ipAddressColumnWidth;
            });
        }

        public void RemoveComments()
        {
            HostEntries = HostEntries.Where(en => !(en is CommentsHostEntry)).ToList();
        }

        public void ToggleState(HostEntry hostEntry)
        {
            var entryIndex = HostEntries.IndexOf(hostEntry);
            HostEntries.RemoveAt(entryIndex);
            HostEntry entry = null;
            if (hostEntry is EnabledHostEntry)
            {
                entry = new DisabledHostEntry();
                entry.CopyFrom(hostEntry);
            }
            else
            {
                entry = new EnabledHostEntry();
                entry.CopyFrom(hostEntry);
            }
            HostEntries.Insert(entryIndex, entry);
            UpdateColumnWidth();
        }
    }
}