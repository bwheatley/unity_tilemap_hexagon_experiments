using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Util;

namespace Util
{
    public class DataDump
    {

        public static void WriteToCSV(string filePath, StringBuilder csv)
        {
            /*
            var csv = new StringBuilder();

            va2r _id = "0";
            var _name = "1st Infantry";
            var _count = "15000";

            Util.WriteDebugLog(string.Format("write to file now!"));


            var _genericLine = string.Format("{0},{1},{2}", _id, _name, _count);

            csv.AppendLine(_genericLine);
            csv.AppendLine(_genericLine);
            csv.AppendLine(_genericLine);
            */

            //We don't neccessarily want a new file, we can append
            //File.WriteAllText(filePath, csv.ToString());
            File.AppendAllText(filePath, csv.ToString());
        }




    } 
}
