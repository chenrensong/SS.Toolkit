using System;
using System.Collections.Generic;
using System.Text;

namespace SS.Toolkit.Helpers
{
    public class StringHelper
    {
        public static string ToUnderlineStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        buf.Append("_");
                    }
                    buf.Append(char.ToLower(c));
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }
    }
}
