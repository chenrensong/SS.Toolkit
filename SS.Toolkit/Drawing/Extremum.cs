using System;
using System.Collections.Generic;
using System.Text;

namespace SS.Toolkit.Drawing
{
    public class Extremum
    {
        public static List<int> GetMinExtrem(int[] data)
        {
            int min = data[0];
            var list = new List<int>();

            int miniSpan = 5; //规定最小跨度
            int lastEqIndex = 0;

            for (int i = 1; i < data.Length - 1; i++)
            {
                if (data[i] > data[i + 1])
                {
                    min = data[i + 1];
                }

                // 保存101的这种类型的极值
                if (data[i] < data[i + 1] && data[i] <= min)
                {
                    //判断两个极小值之间的距离是否满足最小跨度
                    if ((list.Count != 0 && (i - list[list.Count - 1] >= miniSpan)) || (list.Count == 0))
                    {
                        list.Add(i);
                    }
                }

                // 保存100000001这种类型的极值，只取第一个0，和最后一个0为极值点
                if (data[i] == data[i + 1])
                {
                    if (lastEqIndex + 1 != i)
                    {
                        if ((list.Count != 0 && (i - list[list.Count - 1] >= miniSpan)) || (list.Count == 0))
                        {
                            list.Add(i);
                        }
                    }
                    lastEqIndex = i;
                }
            }
            foreach (var integer in list)
            {
                System.Console.WriteLine(integer + " ");
            }
            return list;
        }
    }
}
