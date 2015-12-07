using System;
using System.Collections.Generic;
using System.Text;

namespace hix
{
    class Util
    {
        public static string[] Tail(string[] xs)
        {
            string[] x = new string[xs.Length - 1];
            for (int c = 0; c < xs.Length - 1; c++)
            {
                x[c] = xs[c + 1];
            }
            return x;
        }       
    }
}
