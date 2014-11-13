using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JShibo.Serialization.Csv
{
    public class CsvStringSize
    {
        #region 字段

        int size = 0;

        internal int Size
        {
            get { return size; }
        }

        #endregion

        #region 方法

        internal void Write(string value)
        {
            if (value != null)
                size += value.Length + 3;
            else
                size += 7;
        }

        internal void Write(string[] value)
        {
            if (value != null)
                size += value.Length + 3;
            else
                size += 7;
        }

        internal void Write(Uri value)
        {
            if (value != null)
                size += value.AbsoluteUri.Length + 3;
            else
                size += 7;
        }

        internal void Write(IList value)
        {

        }





        internal void Write(IList<bool> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 8;
        }

        internal void Write(IList<char> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 4;
        }

        internal void Write(IList<byte> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 4;
        }

        internal void Write(IList<sbyte> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 5;
        }

        internal void Write(IList<short> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 6;
        }

        internal void Write(IList<ushort> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 7;
        }

        internal void Write(IList<int> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 12;
        }

        internal void Write(IList<uint> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 11;
        }

        internal void Write(IList<long> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 21;
        }

        internal void Write(IList<ulong> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 20;
        }

        internal void Write(IList<float> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 11;
        }

        internal void Write(IList<double> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 20;
        }

        internal void Write(IList<decimal> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 20;
        }

        internal void Write(IList<string> value)
        {
            if (value == null)
                size += 7;
            else
            {
                foreach (string s in value)
                {
                    if (s != null)
                        size += s.Length + 3;
                    else
                        size += 7;
                }
            }
        }

        internal void Write(IList<DateTime> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 36;
        }

        internal void Write(IList<DateTimeOffset> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 20;
        }

        internal void Write(IList<TimeSpan> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 20;
        }

        internal void Write(IList<Guid> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 39;
        }

        internal void Write(IList<Enum> value)
        {
            if (value == null)
                size += 7;
            else
                size += value.Count * 10;
        }

        internal void Write(IList<Uri> value)
        {
            if (value == null)
                size += 7;
            else
            {
                foreach (Uri s in value)
                {
                    if (s != null)
                        size += s.AbsoluteUri.Length + 3;
                    else
                        size += 7;
                }
            }
        }



        #endregion
    }
}
