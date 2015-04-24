/*
 *
 * author: jbuddha
 * timestamp: 4/23/2015 11:29 PM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace JarScanner
{
    /// <summary>
    /// Description of Keeper.
    /// </summary>
    /// 
    [Serializable]
    public class Keeper
    {
        public Dictionary<string, ArrayList> Entries {
            get;
            internal set;
        }
        
        public int TotalJars {get; internal set;}
        public int TotalFiles {get; internal set;}
        
        public Keeper()
        {
        }
        
        
        
        public string ToString()
        {
            var sb = new StringBuilder();
            foreach(KeyValuePair<string, ArrayList> fileEntry in Entries){
                sb.AppendLine(Environment.NewLine + fileEntry.Key);
                foreach(string entry in fileEntry.Value){
                    sb.AppendLine("    " + entry);
                }
            }
            if(sb.Length > 10000)
                return sb.ToString().Substring(0, 10000) + Environment.NewLine+ "... many more results";
            else
                return sb.ToString();
        }
        
        public string ToString(string text)
        {
            var sb = new StringBuilder();
            foreach(KeyValuePair<string, ArrayList> fileEntry in Entries){
                var partialSb = new StringBuilder();
                partialSb.AppendLine(Environment.NewLine + fileEntry.Key);
                int count = 0;
                foreach(string entry in fileEntry.Value){
                    if (entry.IndexOf(text,StringComparison.OrdinalIgnoreCase)>=0) {
                        partialSb.AppendLine("    " + entry);
                        count++;
                    }
                }
                if (count > 0)
                    sb.Append(partialSb);
            }
            if(sb.Length > 10000)
                return sb.ToString().Substring(0, 10000) + Environment.NewLine+"... many more results";
            else
                return sb.ToString();
        }
        

    }
}
