using System;
using System.Collections;
using System.Text;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace PTGDK.Utility
{
    public static class PrintUtils
    {
        /// <summary>
        /// Print a list as a single line 
        /// </summary>
        /// <param name="list">List to print</param>
        public static void PrintList(IList<object> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (object o in list)
            {
                stringBuilder.Append(o.ToString()).Append(" ");
            }
            Console.WriteLine(stringBuilder.ToString());
        }
        
    }
}