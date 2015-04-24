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
                    if (entry.Contains(text)) {
                        partialSb.AppendLine("    " + entry);
                        count++;
                    }
                }
                if (count > 0)
                    sb.Append(partialSb);
            }
            return sb.ToString();
        }
    }
}
