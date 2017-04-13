// Implementation based on script found at http://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/ 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CSVReader
{
    const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    const string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static readonly char[] TRIM_CHARS = { '\"' };
    
    public static List<Dictionary<string, string>> Read(TextAsset data)
    {
        List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();

        string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);
        
        if(lines.Length <= 1)
        {
            return returnList;
        }
        
        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for(int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if((values.Length == 0) || (string.IsNullOrEmpty(values[0]) == true))
            {
                continue;
            }
            
            Dictionary<string, string> entry = new Dictionary<string, string>();
            for(int j = 0; (j < header.Length) && (j < values.Length); j++ )
            {
                string value = values[j];

                // Remove the first and last quotes
                value = value.TrimStart(TRIM_CHARS);
                value = value.TrimEnd(TRIM_CHARS);

                // FIXME: why are we removing backlashes?
                value = value.Replace("\\", "");

                // Update entry
                entry[header[j]] = value;
            }
            returnList.Add (entry);
        }
        return returnList;
    }
}