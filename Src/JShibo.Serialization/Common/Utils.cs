﻿using JShibo.Serialization.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
//using System.Linq;
using System.Reflection;
using System.Text;

namespace JShibo.Serialization.Common
{
    public static class Utils
    {
        #region 字段

        const int LengthFromLargestChar = '\\' + 1;

        static readonly char[] base64Table = new char[] 
        { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/', 
            '='
        };

        static readonly char[] EscapeChars = new[]
		{
			'\'', '\n', '\r', '\t', '"', '\\', '\f', '\b',
		};
		
		static readonly bool[] EscapeCharFlags = new bool[LengthFromLargestChar];

        internal static char[] LowerChars = null;

        static Utils()
		{
			foreach (char escapeChar in EscapeChars)
			{
				EscapeCharFlags[escapeChar] = true;
			}
            LowerChars = GetLowerCharsIndex();
		}

        static char[] GetLowerCharsIndex()
        {
            if (LowerChars != null)
                return LowerChars;

            LowerChars = new char[65536];
            for (int i = 0; i < 65536; i++)
            {
                if (char.IsLetter((char)i) == true)
                    LowerChars[i] = char.ToLower((char)i);
                else
                    LowerChars[i] = (char)i;
            }
            return LowerChars;
        }

        #endregion

        #region 序列化相关

        /// <summary>
        /// 数据是否是值类型，这样便可以使用值类型的序列化，获得更好的性能。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean IsFastSerializeable(object value)
        {
            if (value == null)
                return true;

            else if (value is string)
                return true;

            else if (value is Int32)
                return true;

            else if (value == DBNull.Value)
                return true;

            else if (value is Boolean)
                return true;

            else if (value is Decimal)
                return true;

            else if (value is DateTime)
                return true;

            else if (value is Double)
                return true;

            else if (value is Single)
                return true;

            else if (value is Int16)
                return true;

            else if (value is Guid)
                return true;

            else if (value is Int64)
                return true;

            else if (value is Byte)
                return true;

            else if (value is Char)
                return true;

            else if (value is SByte)
                return true;

            else if (value is UInt32)
                return true;

            else if (value is UInt16)
                return true;

            else if (value is UInt64)
                return true;

            else if (value is TimeSpan)
                return true;

            else if (value is Array)
                return true;

            else if (value is Type)
                return true;

            else if (value is BitArray)
                return true;

            else if (value is BitVector32)
                return true;

            //else if (isTypeRecreatable(value.GetType()))
            //    return true;

            //else if (value is SingletonTypeWrapper)
            //    return true;

            else if (value is ArrayList)
                return true;

            else if (value is Enum)
                return true;

            return false;

        }

        internal static bool HasSerializableAttribute(object[] atts)
        {
            foreach (Attribute attr in atts)
            {
                if (attr is SerializableAttribute)
                    return true;
            }
            return false;
        }

        internal static bool IsDeep(Type type)
        {
            if (type.IsGenericType)
                return false;
            if (type.GetInterface("IList") == typeof(IList) ||
                    type.GetInterface("IDictionary") == typeof(IDictionary) ||
                    type.GetInterface("ICollection") == typeof(ICollection))
                return false;
            if (type.IsPrimitive)
                return false;

            if (type == typeof(decimal))
                return false;

            if (type == typeof(ArraySegment<bool>))
                return false;
            if (type == typeof(ArraySegment<byte>))
                return false;
            if (type == typeof(ArraySegment<sbyte>))
                return false;
            if (type == typeof(ArraySegment<short>))
                return false;
            if (type == typeof(ArraySegment<ushort>))
                return false;
            if (type == typeof(ArraySegment<int>))
                return false;
            if (type == typeof(ArraySegment<uint>))
                return false;
            if (type == typeof(ArraySegment<long>))
                return false;
            if (type == typeof(ArraySegment<ulong>))
                return false;
            if (type == typeof(ArraySegment<float>))
                return false;
            if (type == typeof(ArraySegment<double>))
                return false;
            if (type == typeof(ArraySegment<decimal>))
                return false;
            if (type == typeof(ArraySegment<char>))
                return false;
            if (type == typeof(ArraySegment<DateTime>))
                return false;
            if (type == typeof(ArraySegment<DateTimeOffset>))
                return false;
            if (type == typeof(ArraySegment<TimeSpan>))
                return false;
            if (type == typeof(ArraySegment<Uri>))
                return false;
            if (type == typeof(ArraySegment<Guid>))
                return false;



            if (type == typeof(Uri))
                return false;
            if (type == typeof(Enum))
                return false;
            if (type == typeof(DateTime))
                return false;
            if (type == typeof(TimeSpan))
                return false;
            if (type == typeof(DateTimeOffset))
                return false;
            if (type == typeof(Guid))
                return false;
            if (type == typeof(DataTable))
                return false;
            if (type == typeof(DataSet))
                return false;
            if (type.IsClass == true && type != typeof(string) && type.IsArray == false)
            {
                return true;
            }



            return false;
        }

        /// <summary>
        /// 默认原生支持的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsJsonBaseType(Type type)
        {
            if (type.GetInterface("IList") == typeof(IList) ||
                    type.GetInterface("IDictionary") == typeof(IDictionary) ||
                    type.GetInterface("ICollection") == typeof(ICollection))
                return true;
            if (type.IsPrimitive)
                return true;

            if (type == typeof(decimal))
                return true;

            if (type == typeof(ArraySegment<bool>))
                return true;
            if (type == typeof(ArraySegment<byte>))
                return true;
            if (type == typeof(ArraySegment<sbyte>))
                return true;
            if (type == typeof(ArraySegment<short>))
                return true;
            if (type == typeof(ArraySegment<ushort>))
                return true;
            if (type == typeof(ArraySegment<int>))
                return true;
            if (type == typeof(ArraySegment<uint>))
                return true;
            if (type == typeof(ArraySegment<long>))
                return true;
            if (type == typeof(ArraySegment<ulong>))
                return true;
            if (type == typeof(ArraySegment<float>))
                return true;
            if (type == typeof(ArraySegment<double>))
                return true;
            if (type == typeof(ArraySegment<decimal>))
                return true;
            if (type == typeof(ArraySegment<char>))
                return true;
            if (type == typeof(ArraySegment<DateTime>))
                return true;
            if (type == typeof(ArraySegment<DateTimeOffset>))
                return true;
            if (type == typeof(ArraySegment<TimeSpan>))
                return true;
            if (type == typeof(ArraySegment<Uri>))
                return true;
            if (type == typeof(ArraySegment<Guid>))
                return true;

            if (type == typeof(Uri))
                return true;
            if (type == typeof(Enum))
                return true;
            if (type == typeof(DateTime))
                return true;
            if (type == typeof(TimeSpan))
                return true;
            if (type == typeof(DateTimeOffset))
                return true;
            if (type == typeof(Guid))
                return true;
            if (type == typeof(DataTable))
                return true;
            if (type == typeof(DataSet))
                return true;
            return false;
        }

        /// <summary>
        /// 是否是序列化的基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsBaseType(Type type)
        {
            if (type.IsClass)
            {
                if (type.IsGenericType == true)
                {
                    if (type.GetInterface("IList") == typeof(IList) ||
                        type.GetInterface("IDictionary") == typeof(IDictionary) ||
                        type.GetInterface("ICollection") == typeof(ICollection))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
                return true;
        }

        internal static void GetTypes(Type type, List<Type> types)
        {
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (IsDeep(field.FieldType))
                {
                    types.Add(field.FieldType);
                    GetTypes(field.FieldType, types);
                }
            }
            PropertyInfo[] propertys = type.GetProperties();
            foreach (PropertyInfo property in propertys)
            {
                if (IsDeep(property.PropertyType))
                {
                    types.Add(property.PropertyType);
                    GetTypes(property.PropertyType, types);
                }
            }
        }

        internal static void GetNames(Type type, List<string> names)
        {
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                names.Add(Utils.GetAttributeName(field));
                if (IsDeep(field.FieldType))
                    GetNames(field.FieldType, names);
            }
            PropertyInfo[] propertys = type.GetProperties();
            foreach (PropertyInfo property in propertys)
            {
                names.Add(Utils.GetAttributeName(property));
                if (IsDeep(property.PropertyType))
                    GetNames(property.PropertyType, names);
            }
        }

        internal static void GetLength(Type type, ref int minSize)
        {
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (Utils.IsDeep(field.FieldType))
                {
                    GetLength(field.FieldType, ref minSize);
                }
            }
            PropertyInfo[] propertys = type.GetProperties();
            foreach (PropertyInfo property in propertys)
            {
                if (Utils.IsDeep(property.PropertyType))
                {
                    GetLength(property.PropertyType, ref minSize);
                }
            }
        }

        internal static bool IsTypeDecoratedByAttribute<T>(object[] t)
        {
            foreach (Attribute attr in t)
            {
                if (attr is T)
                {
                    //T a = (T)Convert.ChangeType(attr, typeof(T));
                    return true;
                }
            }
            return false;
        }

        internal static bool IsIgnoreAttribute(FieldInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is NotSerialized)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsIgnoreAttribute(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is NotSerialized)
                {
                    return true;
                }
            }
            return false;
        }

        internal static string GetAttributeName(FieldInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is JsonAttribute)
                {
                    if (string.IsNullOrEmpty(((JsonAttribute)att).Name) == false)
                        return ((JsonAttribute)att).Name;
                }
            }
            return info.Name;
        }

        internal static string GetAttributeName(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is JsonAttribute)
                {
                    if (string.IsNullOrEmpty(((JsonAttribute)att).Name) == false)
                        return ((JsonAttribute)att).Name;
                }
            }
            return info.Name;
        }

        internal static CheckAttribute GetCheckAttribute(FieldInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is CheckAttribute)
                {
                    return ((CheckAttribute)att);
                }
            }
            return CheckAttribute.Default;
        }

        internal static CheckAttribute GetCheckAttribute(PropertyInfo info)
        {
            object[] atts = info.GetCustomAttributes(true);
            foreach (Attribute att in atts)
            {
                if (att is CheckAttribute)
                {
                    return ((CheckAttribute)att);
                }
            }
            return CheckAttribute.Default;
        }

        #endregion

        #region 拷贝相关

        internal static unsafe int CheckThenCopy(char* dmem, char* smem, int charCount)
        {
            #region old

            //int addCount = 0;
            //for (int i = 0; i < charCount; i++)
            //{
            //    if (*smem != '"')
            //        *dmem++ = *smem++;
            //    else
            //    {
            //        addCount++;
            //        *dmem++ = '\\';
            //        *dmem++ = *smem++;
            //    }
            //}
            //return addCount;

            #endregion

            #region m1

            int addCount = 0;
            if (charCount > 0)
            {
                while (charCount >= 8)
                {
                    //if (*smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"')
                    if (*smem != '"' && *(smem + 1) != '"' && *(smem + 2) != '"' && *(smem + 3) != '"' && *(smem + 4) != '"' && *(smem + 5) != '"' && *(smem + 6) != '"' && *(smem + 7) != '"')
                    {
                        *((uint*)dmem) = *((uint*)smem);
                        *((uint*)(dmem + 2)) = *((uint*)(smem + 2));
                        *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                        *((uint*)(dmem + 6)) = *((uint*)(smem + 6));
                        dmem += 8;
                        smem += 8;
                        charCount -= 8;
                    }
                    else
                    {
                        //smem -= 8;
                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }

                        if (*smem != '"')
                            *dmem++ = *smem++;
                        else
                        {
                            addCount++;
                            *dmem++ = '\\';
                            *dmem++ = *smem++;
                        }
                    }
                }
                if ((charCount & 4) != 0)
                {
                    //if (*smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"')
                    if (*smem != '"' && *(smem + 1) != '"' && *(smem + 2) != '"' && *(smem + 3) != '"')
                    {
                        //smem -= 4;
                        *((uint*)dmem) = *((uint*)smem);
                        *((uint*)(dmem + 2)) = *((uint*)(smem + 2));
                        dmem += 4;
                        smem += 4;
                    }
                    else
                    {
                        //smem -= 4;
                        for (int i = 0; i < 4; i++)
                        {
                            if (*smem != '"')
                                *dmem++ = *smem++;
                            else
                            {
                                addCount++;
                                *dmem++ = '\\';
                                *dmem++ = *smem++;
                            }
                        }
                    }
                }
                if ((charCount & 2) != 0)
                {
                    //if (*smem++ != '"' && *smem++ != '"')
                    if (*smem != '"' && *(smem + 1) != '"')
                    {
                        //smem -= 2;
                        *((uint*)dmem) = *((uint*)smem);
                        dmem += 2;
                        smem += 2;
                    }
                    else
                    {
                        //smem -= 2;
                        for (int i = 0; i < 2; i++)
                        {
                            if (*smem != '"')
                                *dmem++ = *smem++;
                            else
                            {
                                addCount++;
                                *dmem++ = '\\';
                                *dmem++ = *smem++;
                            }
                        }
                    }
                }
                if ((charCount & 1) != 0)
                {
                    if (*smem != '"')
                        *dmem++ = *smem++;
                    else
                    {
                        addCount++;
                        *dmem++ = '\\';
                        *dmem++ = *smem++;
                    }
                }
            }
            return addCount;

            #endregion

        }

        internal static unsafe int CheckFullThenCopy(char* dmem, char* smem, int charCount)
        {
            #region m2
            //使用case虽然能很好的解决问题，但性能问题比较大
            int addCount = 0;
            if (charCount > 0)
            {
                for (int i = 0; i < charCount; i++)
                {
                    switch (*smem)
                    {
                        case '\t':
                            *dmem++ = '\\';
                            *dmem++ = 't';
                            smem++;
                            addCount++;
                            break;
                        case '\n':
                            *dmem++ = '\\';
                            *dmem++ = 'n';
                            smem++;
                            addCount++;
                            break;
                        case '\r':
                            *dmem++ = '\\';
                            *dmem++ = 'r';
                            smem++;
                            addCount++;
                            break;
                        case '\f':
                            *dmem++ = '\\';
                            *dmem++ = 'f';
                            smem++;
                            addCount++;
                            break;
                        case '\b':
                            *dmem++ = '\\';
                            *dmem++ = 'b';
                            smem++;
                            addCount++;
                            break;
                        case '\\':
                            *dmem++ = '\\';
                            *dmem++ = '\\';
                            smem++;
                            addCount++;
                            break;
                        case '"':
                            *dmem++ = '\\';
                            *dmem++ = '"';
                            smem++;
                            addCount++;
                            break;
                        default:
                            *dmem++ = *smem++;
                            break;
                    }
                }
            }
            return addCount;

            #endregion

            #region m3

            ////使用数组标记提高性能
            //int addCount = 0;
            //if (charCount > 0)
            //{
            //    for (int i = 0; i < charCount; i++)
            //    {
            //        if (*smem >= LengthFromLargestChar || !EscapeCharFlags[*smem])
            //        {
            //            *dmem++ = *smem++;
            //        }
            //        else
            //        {
            //            *dmem++ = '\\';
            //            addCount++;
            //            switch (*smem)
            //            {
            //                case '\t':
            //                    *dmem++ = 't';
            //                    break;
            //                case '\n':
            //                    *dmem++ = 'n';
            //                    break;
            //                case '\r':
            //                    *dmem++ = 'r';
            //                    break;
            //                case '\f':
            //                    *dmem++ = 'f';
            //                    break;
            //                case '\b':
            //                    *dmem++ = 'b';
            //                    break;
            //                case '\\':
            //                    *dmem++ = '\\';
            //                    break;
            //                case '"':
            //                    *dmem++ = '"';
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }
            //    }
            //}
            //return addCount;

            #endregion
        }

        /// <summary>
        /// 系统内部的快速拷贝方法
        /// </summary>
        /// <param name="dmem"></param>
        /// <param name="smem"></param>
        /// <param name="charCount"></param>
        public static unsafe void wstrcpy(char* dmem, char* smem, int charCount)
        {
            if (charCount > 0)
            {
                while (charCount >= 8)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    *((uint*)(dmem + 2)) = *((uint*)(smem + 2));
                    *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                    *((uint*)(dmem + 6)) = *((uint*)(smem + 6));
                    dmem += 8;
                    smem += 8;
                    charCount -= 8;
                }
                if ((charCount & 4) != 0)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    *((uint*)(dmem + 2)) = *((uint*)(smem + 2));
                    dmem += 4;
                    smem += 4;
                }
                if ((charCount & 2) != 0)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    dmem += 2;
                    smem += 2;
                }
                if ((charCount & 1) != 0)
                {
                    *dmem = *smem;
                }
            }

            #region old

            //原始的字符拷贝int和uint指针不能相互转换，进行了部分修改。
            //if (charCount > 0)
            //{
            //    if ((((int)dmem) & 2) != 0)
            //    {
            //        dmem[0] = smem[0];
            //        dmem++;
            //        smem++;
            //        charCount--;
            //    }
            //    while (charCount >= 8)
            //    {
            //        *((int*)dmem) = *((uint*)smem);
            //        *((int*)(dmem + 2)) = *((uint*)(smem + 2));
            //        *((int*)(dmem + 4)) = *((uint*)(smem + 4));
            //        *((int*)(dmem + 6)) = *((uint*)(smem + 6));
            //        dmem += 8;
            //        smem += 8;
            //        charCount -= 8;
            //    }
            //    if ((charCount & 4) != 0)
            //    {
            //        *((int*)dmem) = *((uint*)smem);
            //        *((int*)(dmem + 2)) = *((uint*)(smem + 2));
            //        dmem += 4;
            //        smem += 4;
            //    }
            //    if ((charCount & 2) != 0)
            //    {
            //        *((int*)dmem) = *((uint*)smem);
            //        dmem += 2;
            //        smem += 2;
            //    }
            //    if ((charCount & 1) != 0)
            //    {
            //        dmem[0] = smem[0];
            //    }
            //}

            #endregion
        }

        internal static unsafe void wstrcpy(byte* dmem, byte* smem, int byteCount)
        {
            if (byteCount > 0)
            {
                while (byteCount >= 16)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                    *((uint*)(dmem + 8)) = *((uint*)(smem + 8));
                    *((uint*)(dmem + 12)) = *((uint*)(smem + 12));
                    dmem += 16;
                    smem += 16;
                    byteCount -= 16;
                }
                if ((byteCount & 8) != 0)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                    dmem += 8;
                    smem += 8;
                }
                if ((byteCount & 4) != 0)
                {
                    *((uint*)dmem) = *((uint*)smem);
                    dmem += 4;
                    smem += 4;
                }
                if ((byteCount & 2) != 0)
                {
                    *((ushort*)dmem) = *((ushort*)smem);
                    dmem += 2;
                    smem += 2;
                }
                if ((byteCount & 1) != 0)
                {
                    *dmem = *smem;
                }
            }
        }

        internal static unsafe void FastCopyGuid(char* dmem, char* smem)
        {
            *((uint*)dmem) = *((uint*)smem);
            *((uint*)(dmem + 2)) = *((uint*)(smem + 2));
            *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
            *((uint*)(dmem + 6)) = *((uint*)(smem + 6));
            *((uint*)(dmem + 8)) = *((uint*)(smem + 8));
            *((uint*)(dmem + 10)) = *((uint*)(smem + 10));
            *((uint*)(dmem + 12)) = *((uint*)(smem + 12));
            *((uint*)(dmem + 14)) = *((uint*)(smem + 14));
            *((uint*)(dmem + 16)) = *((uint*)(smem + 16));
            *((uint*)(dmem + 18)) = *((uint*)(smem + 18));
            *((uint*)(dmem + 20)) = *((uint*)(smem + 20));
            *((uint*)(dmem + 22)) = *((uint*)(smem + 22));
            *((uint*)(dmem + 24)) = *((uint*)(smem + 24));
            *((uint*)(dmem + 26)) = *((uint*)(smem + 26));
            *((uint*)(dmem + 28)) = *((uint*)(smem + 28));
            *((uint*)(dmem + 30)) = *((uint*)(smem + 30));
            *((uint*)(dmem + 32)) = *((uint*)(smem + 32));
            *((uint*)(dmem + 34)) = *((uint*)(smem + 34));
        }

        /// <summary>
        /// 目前暂时测试，性能总比IndexOf低
        /// </summary>
        /// <param name="smem"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        internal static unsafe bool FastFindEscape(char* smem, int charCount)
        {
            if (charCount > 0)
            {
                while (charCount >= 8)
                {
                    //if (*smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"')
                    //    charCount -= 8;
                    if (*smem != '"' && *(smem + 1) != '"' && *(smem + 2) != '"' && *(smem + 3) != '"' && *(smem + 4) != '"' && *(smem + 5) != '"' && *(smem + 6) != '"' && *(smem + 7) != '"')
                    {
                        charCount -= 8;
                        smem += 8;
                    }
                    else
                        return true;
                }
                if ((charCount & 4) != 0)
                {
                    if (*smem++ != '"' && *smem++ != '"' && *smem++ != '"' && *smem++ != '"')
                        ;
                    else
                        return true;
                }
                if ((charCount & 2) != 0)
                {
                    if (*smem++ == '"' || *smem++ == '"')
                        ;
                    else
                        return true;
                }
                if ((charCount & 1) != 0)
                {
                    if (*smem++ == '"')
                        ;
                    else
                        return true;
                }
            }
            return false;
        }

        internal unsafe static int ToCharAsUnicode(char* dmem, char* smem, int charCount)
        {
            int tv = 0, ecount = 0;
            while (charCount > 0)
            {
                if (*smem < 128)
                    *dmem++ = *smem++;
                else
                {
                    *dmem++ = '\\';
                    *dmem++ = 'u';
                    //*dmem++ = IntToHex((*smem >> 12) & '\x000f');
                    //*dmem++ = IntToHex((*smem >> 8) & '\x000f');
                    //*dmem++ = IntToHex((*smem >> 4) & '\x000f');
                    //*dmem++ = IntToHex(*smem++ & '\x000f');

                    *dmem++ = (tv = ((*smem >> 12) & '\x000f')) <= 9 ? (char)(tv + 48) : (char)(tv + 87);
                    *dmem++ = (tv = ((*smem >> 8) & '\x000f')) <= 9 ? (char)(tv + 48) : (char)(tv + 87);
                    *dmem++ = (tv = ((*smem >> 4) & '\x000f')) <= 9 ? (char)(tv + 48) : (char)(tv + 87);
                    *dmem++ = (tv = ((*smem++) & '\x000f')) <= 9 ? (char)(tv + 48) : (char)(tv + 87);
                    ecount++;
                }
                charCount--;
            }
            return ecount;
        }

        internal static unsafe int ConvertToBase64Array(char* outChars, byte* inData, int offset, int length, bool insertLineBreaks)
        {
            int num = length % 3;
            int num2 = offset + (length - num);
            int index = 0;
            int num4 = 0;
            fixed (char* chRef = base64Table)
            {
                int num5;
                for (num5 = offset; num5 < num2; num5 += 3)
                {
                    if (insertLineBreaks)
                    {
                        if (num4 == 0x4c)
                        {
                            outChars[index++] = '\r';
                            outChars[index++] = '\n';
                            num4 = 0;
                        }
                        num4 += 4;
                    }
                    outChars[index] = chRef[(inData[num5] & 0xfc) >> 2];
                    outChars[index + 1] = chRef[((inData[num5] & 3) << 4) | ((inData[num5 + 1] & 240) >> 4)];
                    outChars[index + 2] = chRef[((inData[num5 + 1] & 15) << 2) | ((inData[num5 + 2] & 0xc0) >> 6)];
                    outChars[index + 3] = chRef[inData[num5 + 2] & 0x3f];
                    index += 4;
                }
                num5 = num2;
                if ((insertLineBreaks && (num != 0)) && (num4 == 0x4c))
                {
                    outChars[index++] = '\r';
                    outChars[index++] = '\n';
                }
                switch (num)
                {
                    case 1:
                        outChars[index] = chRef[(inData[num5] & 0xfc) >> 2];
                        outChars[index + 1] = chRef[(inData[num5] & 3) << 4];
                        outChars[index + 2] = chRef[0x40];
                        outChars[index + 3] = chRef[0x40];
                        index += 4;
                        break;

                    case 2:
                        outChars[index] = chRef[(inData[num5] & 0xfc) >> 2];
                        outChars[index + 1] = chRef[((inData[num5] & 3) << 4) | ((inData[num5 + 1] & 240) >> 4)];
                        outChars[index + 2] = chRef[(inData[num5 + 1] & 15) << 2];
                        outChars[index + 3] = chRef[0x40];
                        index += 4;
                        break;
                }
            }
            return index;
        }

        internal static unsafe int ConvertToBase64Array(byte* outChars, byte* inData, int offset, int length, bool insertLineBreaks)
        {
            int num = length % 3;
            int num2 = offset + (length - num);
            int index = 0;
            int num4 = 0;
            fixed (char* chRef = base64Table)
            {
                int num5;
                for (num5 = offset; num5 < num2; num5 += 3)
                {
                    if (insertLineBreaks)
                    {
                        if (num4 == 0x4c)
                        {
                            outChars[index++] = (byte)'\r';
                            outChars[index++] = (byte)'\n';
                            num4 = 0;
                        }
                        num4 += 4;
                    }
                    outChars[index] = (byte)chRef[(inData[num5] & 0xfc) >> 2];
                    outChars[index + 1] = (byte)chRef[((inData[num5] & 3) << 4) | ((inData[num5 + 1] & 240) >> 4)];
                    outChars[index + 2] = (byte)chRef[((inData[num5 + 1] & 15) << 2) | ((inData[num5 + 2] & 0xc0) >> 6)];
                    outChars[index + 3] = (byte)chRef[inData[num5 + 2] & 0x3f];
                    index += 4;
                }
                num5 = num2;
                if ((insertLineBreaks && (num != 0)) && (num4 == 0x4c))
                {
                    outChars[index++] = (byte)'\r';
                    outChars[index++] = (byte)'\n';
                }
                switch (num)
                {
                    case 1:
                        outChars[index] = (byte)chRef[(inData[num5] & 0xfc) >> 2];
                        outChars[index + 1] = (byte)chRef[(inData[num5] & 3) << 4];
                        outChars[index + 2] = (byte)chRef[0x40];
                        outChars[index + 3] = (byte)chRef[0x40];
                        index += 4;
                        break;

                    case 2:
                        outChars[index] = (byte)chRef[(inData[num5] & 0xfc) >> 2];
                        outChars[index + 1] = (byte)chRef[((inData[num5] & 3) << 4) | ((inData[num5 + 1] & 240) >> 4)];
                        outChars[index + 2] = (byte)chRef[(inData[num5 + 1] & 15) << 2];
                        outChars[index + 3] = (byte)chRef[0x40];
                        index += 4;
                        break;
                }
            }
            return index;
        }

        internal static bool IsDigitSwitch(char ch)
        {
            switch (ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }

        internal unsafe static bool HasAnyEscapeChars(char* buffer, int len)
        {
            for (var i = 0; i < len; i++)
            {
                if (*buffer >= LengthFromLargestChar || !EscapeCharFlags[*buffer]) continue;
                return true;
            }
            return false;
        }

        internal static T[] Resize<T>(T[] buffer,int shift)
        {
            T[] result = new T[buffer.Length << 1];
            Buffer.BlockCopy(buffer, 0, result, 0, buffer.Length << shift);
            return result;
        }

        internal static T[] ToArray<T>(T[] buffer,int offset,int size,int shift)
        {
            T[] result = new T[size];
            Buffer.BlockCopy(buffer, offset << shift, result, 0, size << shift);
            return result;
        }

        internal static short[] ToArrayShort(int[] buffer, int offset, int size)
        {
            short[] result = new short[size];
            for (int i = offset; i < offset + size; i++)
                result[i] = (short)buffer[i];
            return result;
        }

        internal static List<short> ToListShort(int[] buffer, int offset, int size)
        {
            List<short> result = new List<short>(size);
            for (int i = offset; i < offset + size; i++)
                result.Add((short)buffer[i]);
            return result;
        }

        internal static ushort[] ToArrayUShort(uint[] buffer, int offset, int size)
        {
            ushort[] result = new ushort[size];
            for (int i = offset; i < offset + size; i++)
                result[i] = (ushort)buffer[i];
            return result;
        }

        internal static List<ushort> ToListUShort(uint[] buffer, int offset, int size)
        {
            List<ushort> result = new List<ushort>(size);
            for (int i = offset; i < offset + size; i++)
                result.Add((ushort)buffer[i]);
            return result;
        }

        internal static byte[] ToArrayByte(uint[] buffer, int offset, int size)
        {
            byte[] result = new byte[size];
            for (int i = offset; i < offset + size; i++)
                result[i] = (byte)buffer[i];
            return result;
        }

        internal static List<byte> ToListByte(uint[] buffer, int offset, int size)
        {
            List<byte> result = new List<byte>(size);
            for (int i = offset; i < offset + size; i++)
                result.Add((byte)buffer[i]);
            return result;
        }

        internal static sbyte[] ToArraySByte(int[] buffer, int offset, int size)
        {
            sbyte[] result = new sbyte[size];
            for (int i = offset; i < offset + size; i++)
                result[i] = (sbyte)buffer[i];
            return result;
        }

        internal static List<sbyte> ToListSByte(int[] buffer, int offset, int size)
        {
            List<sbyte> result = new List<sbyte>(size);
            for (int i = offset; i < offset + size; i++)
                result.Add((sbyte)buffer[i]);
            return result;
        }

        #endregion

        #region 字符串字节转换

        internal unsafe static void StringAsAscii(byte[] buffer, int pos, string value)
        {
            fixed (byte* pd = &buffer[pos])
            {
                fixed (char* src = value)
                {
                    byte* tpd = pd;
                    char* tsrc = src;
                    for (int i = 0; i < value.Length; i += 4)
                    {
                        *tpd++ = (byte)(*tsrc++);
                        *tpd++ = (byte)(*tsrc++);
                        *tpd++ = (byte)(*tsrc++);
                        *tpd++ = (byte)(*tsrc++);
                    }
                }
            }
        }

        internal unsafe static void StringAsUnicode(byte[] buffer, int pos, string value)
        {
            fixed (byte* pd = &buffer[pos])
            {
                fixed (char* src = value)
                {
                    byte* dmem = pd;
                    char* smem = src;
                    int charCount = value.Length;
                    while (charCount >= 8)
                    {
                        *((uint*)dmem) = *((uint*)smem);
                        *((uint*)(dmem + 4)) = *((uint*)(smem + 2));
                        *((uint*)(dmem + 8)) = *((uint*)(smem + 4));
                        *((uint*)(dmem + 12)) = *((uint*)(smem + 6));
                        dmem += 16;
                        smem += 8;
                        charCount -= 8;
                    }
                    if ((charCount & 4) != 0)
                    {
                        *((uint*)dmem) = *((uint*)smem);
                        *((uint*)(dmem + 4)) = *((uint*)(smem + 2));
                        dmem += 8;
                        smem += 4;
                    }
                    if ((charCount & 2) != 0)
                    {
                        *((uint*)dmem) = *((uint*)smem);
                        dmem += 4;
                        smem += 2;
                    }
                    if ((charCount & 1) != 0)
                    {
                        *((char*)dmem) = *((char*)smem);
                        dmem += 2;
                        smem += 1;
                    }


                    //for (int i = 0; i < value.Length; i += 4)
                    //{
                    //    //*tpd++ = (byte)(*tsrc++);
                    //    //*tpd++ = (byte)(*tsrc++);
                    //    //*tpd++ = (byte)(*tsrc++);
                    //    //*tpd++ = (byte)(*tsrc++);

                    //    *dmem++ = (byte)(*smem);
                    //    *dmem++ = (byte)((*smem) >> 8);
                    //    *dmem++ = (byte)(*(smem + 1));
                    //    *dmem++ = (byte)(*((smem + 1)) >> 8);
                    //    *dmem++ = (byte)(*(smem + 2));
                    //    *dmem++ = (byte)(*((smem + 2)) >> 8);
                    //    *dmem++ = (byte)(*(smem + 3));
                    //    *dmem++ = (byte)(*((smem + 3)) >> 8);
                    //    smem += 4;
                    //}
                }
            }
        }

        internal unsafe static string AsciiAsString(byte[] buffer, int pos, int size)
        {
            char[] value = new char[size];
            fixed (byte* pd = &buffer[pos])
            {
                fixed (char* src = value)
                {
                    byte* tpd = pd;
                    char* tsrc = src;
                    for (int i = 0; i < value.Length; i += 4)
                    {
                        *tsrc++ = (char)(*tpd++);
                        *tsrc++ = (char)(*tpd++);
                        *tsrc++ = (char)(*tpd++);
                        *tsrc++ = (char)(*tpd++);
                    }
                    return new string(tsrc, 0, size);
                }
            }
        }

        internal unsafe static string UnicodeAsString(byte[] buffer, int pos, string value)
        {
            return null;
        }

        #endregion

        #region 写入长度

        internal static void WriteVLong1(byte[] by, int id, ref int pos)
        {
            if (id < 128)
            {
                by[pos] = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                by[pos] = (byte)((id >> 7) | 0x80);
                by[pos + 1] = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 14) | 0x80);
                by[pos + 1] = (byte)((id >> 7) | 0x80);
                by[pos + 2] = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 21) | 0x80);
                by[pos + 1] = (byte)((id >> 14) | 0x80);
                by[pos + 2] = (byte)((id >> 7) | 0x80);
                by[pos + 3] = (byte)(id & 0x7F);
                pos += 4;
            }
            else
            {
                by[pos] = (byte)((id >> 28) | 0x80);
                by[pos + 1] = (byte)((id >> 21) | 0x80);
                by[pos + 2] = (byte)((id >> 14) | 0x80);
                by[pos + 3] = (byte)((id >> 7) | 0x80);
                by[pos + 4] = (byte)(id & 0x7F);
                pos += 5;
            }
        }

        internal static void WriteVLong1(byte[] by, uint id, ref int pos)
        {
            if (id < 128)
            {
                by[pos] = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                by[pos] = (byte)((id >> 7) | 0x80);
                by[pos + 1] = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 14) | 0x80);
                by[pos + 1] = (byte)((id >> 7) | 0x80);
                by[pos + 2] = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 21) | 0x80);
                by[pos + 1] = (byte)((id >> 14) | 0x80);
                by[pos + 2] = (byte)((id >> 7) | 0x80);
                by[pos + 3] = (byte)(id & 0x7F);
                pos += 4;
            }
            else
            {
                by[pos] = (byte)((id >> 28) | 0x80);
                by[pos + 1] = (byte)((id >> 21) | 0x80);
                by[pos + 2] = (byte)((id >> 14) | 0x80);
                by[pos + 3] = (byte)((id >> 7) | 0x80);
                by[pos + 4] = (byte)(id & 0x7F);
                pos += 5;
            }
        }

        internal static void WriteVLong1(byte[] by, long id, ref int pos)
        {
            if (id < 128)
            {
                by[pos] = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                by[pos] = (byte)((id >> 7) | 0x80);
                by[pos + 1] = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 14) | 0x80);
                by[pos + 1] = (byte)((id >> 7) | 0x80);
                by[pos + 2] = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 21) | 0x80);
                by[pos + 1] = (byte)((id >> 14) | 0x80);
                by[pos + 2] = (byte)((id >> 7) | 0x80);
                by[pos + 3] = (byte)(id & 0x7F);
                pos += 4;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L)
            {
                by[pos] = (byte)((id >> 28) | 0x80);
                by[pos + 1] = (byte)((id >> 21) | 0x80);
                by[pos + 2] = (byte)((id >> 14) | 0x80);
                by[pos + 3] = (byte)((id >> 7) | 0x80);
                by[pos + 4] = (byte)(id & 0x7F);
                pos += 5;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128)
            {
                by[pos] = (byte)((id >> 35) | 0x80);
                by[pos + 1] = (byte)((id >> 28) | 0x80);
                by[pos + 2] = (byte)((id >> 21) | 0x80);
                by[pos + 3] = (byte)((id >> 14) | 0x80);
                by[pos + 4] = (byte)((id >> 7) | 0x80);
                by[pos + 5] = (byte)(id & 0x7F);
                pos += 6;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128)
            {
                by[pos] = (byte)((id >> 42) | 0x80);
                by[pos + 1] = (byte)((id >> 35) | 0x80);
                by[pos + 1] = (byte)((id >> 28) | 0x80);
                by[pos + 2] = (byte)((id >> 21) | 0x80);
                by[pos + 3] = (byte)((id >> 14) | 0x80);
                by[pos + 4] = (byte)((id >> 7) | 0x80);
                by[pos + 3] = (byte)((id >> 14) | 0x80);
                by[pos + 4] = (byte)((id >> 7) | 0x80);
                by[pos + 5] = (byte)(id & 0x7F);
                pos += 6;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 49) | 0x80);
                by[pos + 1] = (byte)((id >> 42) | 0x80);
                by[pos + 2] = (byte)((id >> 35) | 0x80);
                by[pos + 3] = (byte)((id >> 28) | 0x80);
                by[pos + 4] = (byte)((id >> 21) | 0x80);
                by[pos + 5] = (byte)((id >> 14) | 0x80);
                by[pos + 6] = (byte)((id >> 7) | 0x80);
                by[pos + 7] = (byte)(id & 0x7F);
                pos += 8;
            }
            else
            {
                by[pos] = (byte)((id >> 56) | 0x80);
                by[pos + 1] = (byte)((id >> 49) | 0x80);
                by[pos + 2] = (byte)((id >> 42) | 0x80);
                by[pos + 3] = (byte)((id >> 35) | 0x80);
                by[pos + 4] = (byte)((id >> 28) | 0x80);
                by[pos + 5] = (byte)((id >> 21) | 0x80);
                by[pos + 6] = (byte)((id >> 14) | 0x80);
                by[pos + 7] = (byte)((id >> 7) | 0x80);
                by[pos + 8] = (byte)(id & 0x7F);
                pos += 9;
            }
        }

        internal static void WriteVLong1(byte[] by, ulong id, ref int pos)
        {
            if (id < 128)
            {
                by[pos] = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                by[pos] = (byte)((id >> 7) | 0x80);
                by[pos + 1] = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 14) | 0x80);
                by[pos + 1] = (byte)((id >> 7) | 0x80);
                by[pos + 2] = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 21) | 0x80);
                by[pos + 1] = (byte)((id >> 14) | 0x80);
                by[pos + 2] = (byte)((id >> 7) | 0x80);
                by[pos + 3] = (byte)(id & 0x7F);
                pos += 4;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L)
            {
                by[pos] = (byte)((id >> 28) | 0x80);
                by[pos + 1] = (byte)((id >> 21) | 0x80);
                by[pos + 2] = (byte)((id >> 14) | 0x80);
                by[pos + 3] = (byte)((id >> 7) | 0x80);
                by[pos + 4] = (byte)(id & 0x7F);
                pos += 5;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128)
            {
                by[pos] = (byte)((id >> 35) | 0x80);
                by[pos + 1] = (byte)((id >> 28) | 0x80);
                by[pos + 2] = (byte)((id >> 21) | 0x80);
                by[pos + 3] = (byte)((id >> 14) | 0x80);
                by[pos + 4] = (byte)((id >> 7) | 0x80);
                by[pos + 5] = (byte)(id & 0x7F);
                pos += 6;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128)
            {
                by[pos] = (byte)((id >> 42) | 0x80);
                by[pos + 1] = (byte)((id >> 35) | 0x80);
                by[pos + 2] = (byte)((id >> 28) | 0x80);
                by[pos + 3] = (byte)((id >> 21) | 0x80);
                by[pos + 4] = (byte)((id >> 14) | 0x80);
                by[pos + 5] = (byte)((id >> 7) | 0x80);
                by[pos + 6] = (byte)(id & 0x7F);
                pos += 7;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128 * 128)
            {
                by[pos] = (byte)((id >> 49) | 0x80);
                by[pos + 1] = (byte)((id >> 42) | 0x80);
                by[pos + 2] = (byte)((id >> 35) | 0x80);
                by[pos + 3] = (byte)((id >> 28) | 0x80);
                by[pos + 4] = (byte)((id >> 21) | 0x80);
                by[pos + 5] = (byte)((id >> 14) | 0x80);
                by[pos + 6] = (byte)((id >> 7) | 0x80);
                by[pos + 7] = (byte)(id & 0x7F);
                pos += 8;
            }
            else
            {
                by[pos] = (byte)((id >> 56) | 0x80);
                by[pos + 1] = (byte)((id >> 49) | 0x80);
                by[pos + 2] = (byte)((id >> 42) | 0x80);
                by[pos + 3] = (byte)((id >> 35) | 0x80);
                by[pos + 4] = (byte)((id >> 28) | 0x80);
                by[pos + 5] = (byte)((id >> 21) | 0x80);
                by[pos + 6] = (byte)((id >> 14) | 0x80);
                by[pos + 7] = (byte)((id >> 7) | 0x80);
                by[pos + 8] = (byte)(id & 0x7F);
                pos += 9;
            }
        }

        internal static int ReadVLong1Int(byte[] b, ref int index)
        {
            byte by = b[index];
            int result;
            if (by < 128)
            {
                result = (by & 0x7F);
                index++;
            }
            else if (b[index + 1] < 128)
            {
                result = ((int)(by & 0x7F) << 7) | (int)(b[index + 1] & 0x7F);
                index += 2;
            }
            else if (b[index + 2] < 128)
            {
                result = ((int)(by & 0x7F) << 14) | ((int)(b[index + 1] & 0x7F) << 7) | (int)(b[index + 2] & 0x7F);
                index += 3;
            }
            else if (b[index + 3] < 128)
            {
                result = ((int)(by & 0x7F) << 21) | ((int)(b[index + 1] & 0x7F) << 14) | ((int)(b[index + 2] & 0x7F) << 7) | (int)(b[index + 3] & 0x7F);
                index += 4;
            }
            else if (b[index + 4] < 128)
            {
                result = ((int)(by & 0x7F) << 28) | ((int)(b[index + 1] & 0x7F) << 21) | ((int)(b[index + 2] & 0x7F) << 14) | ((int)(b[index + 3] & 0x7F) << 7) | (int)(b[index + 4] & 0x7F);
                index += 5;
            }
            else
                throw new Exception("数据太大，无法识别！");
            return result;
        }

        public static uint ReadVLong1UInt(byte[] b, ref int index)
        {
            uint result;
            if (b[index] < 128)
            {
                result = (uint)(b[index] & 0x7F);
                index++;
            }
            else if (b[index + 1] < 128)
            {
                result = ((uint)(b[index] & 0x7F) << 7) | (uint)(b[index + 1] & 0x7F);
                index += 2;
            }
            else if (b[index + 2] < 128)
            {
                result = ((uint)(b[index] & 0x7F) << 14) | ((uint)(b[index + 1] & 0x7F) << 7) | (uint)(b[index + 2] & 0x7F);
                index += 3;
            }
            else if (b[index + 3] < 128)
            {
                result = ((uint)(b[index] & 0x7F) << 21) | ((uint)(b[index + 1] & 0x7F) << 14) | ((uint)(b[index + 2] & 0x7F) << 7) | (uint)(b[index + 3] & 0x7F);
                index += 4;
            }
            else
                throw new Exception("数据太大，无法识别！");
            return result;
        }

        internal static void WriteSize(byte[] buffer, int size, ref int index)
        {
            if (size < 64)
            {
                buffer[index] = (byte)(size & 0x3F);
                index++;
            }
            else if (size < 64 * 128)
            {
                buffer[index] = (byte)((size >> 6) | 0x80);
                buffer[index + 1] = (byte)(size & 0x3F);
                index += 2;
            }
            else if (size < 64 * 128 * 128)
            {
                buffer[index] = (byte)((size >> 13) | 0x80);
                buffer[index + 1] = (byte)((size >> 6) | 0x80);
                buffer[index + 2] = (byte)(size & 0x3F);
                index += 3;
            }
            else if (size < 64 * 128 * 128 * 128)
            {
                buffer[index] = (byte)((size >> 20) | 0x80);
                buffer[index + 1] = (byte)((size >> 13) | 0x80);
                buffer[index + 2] = (byte)((size >> 6) | 0x80);
                buffer[index + 3] = (byte)(size & 0x3F);
                index += 4;
            }
            else
            {
                buffer[index] = (byte)((size >> 27) | 0x80);
                buffer[index + 1] = (byte)((size >> 20) | 0x80);
                buffer[index + 2] = (byte)((size >> 13) | 0x80);
                buffer[index + 3] = (byte)((size >> 6) | 0x80);
                buffer[index + 4] = (byte)(size & 0x3F);
                index += 5;
            }
        }

        internal static int ReadSize(byte[] buffer, ref int index)
        {
            byte b = buffer[index];
            int result;
            if (b < 64)
            {
                result = (b & 0x3F);
                index++;
            }
            else if (buffer[index + 1] < 128)
            {
                result = ((int)(b & 0x7F) << 6) | (int)(buffer[index + 1] & 0x3F);
                index += 2;
            }
            else if (buffer[index + 2] < 128)
            {
                result = ((int)(b & 0x7F) << 13) | ((int)(buffer[index + 1] & 0x7F) << 6) | (int)(buffer[index + 2] & 0x3F);
                index += 3;
            }
            else if (buffer[index + 3] < 128)
            {
                result = ((int)(b & 0x7F) << 20) | ((int)(buffer[index + 1] & 0x7F) << 13) | ((int)(buffer[index + 2] & 0x7F) << 6) | (int)(buffer[index + 3] & 0x3F);
                index += 4;
            }
            else if (buffer[index + 4] < 128)
            {
                result = ((int)(b & 0x7F) << 27) | ((int)(buffer[index + 1] & 0x7F) << 20) | ((int)(buffer[index + 2] & 0x7F) << 13) | ((int)(buffer[index + 3] & 0x7F) << 6) | (int)(buffer[index + 4] & 0x3F);
                index += 5;
            }
            else
                throw new Exception("数据太大，无法识别！");
            return result;
        }

        public static int GetSize(int size)
        {
            if (size < 64)
                return 1;
            else if (size < 64 * 128)
                return 2;
            else if (size < 64 * 128 * 128)
                return 3;
            else if (size < 64 * 128 * 128 * 128)
                return 4;
            else
                return 5;
        }

        #endregion

        #region 写入长度（指针）

        internal unsafe static byte* WriteVLong1(byte* by, int id, ref int pos)
        {
            if (id < 128)
            {
                *by++ = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 4;
            }
            else
            {
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 5;
            }
            return by;
        }

        internal unsafe static byte* WriteVLong1(byte* by, uint id, ref int pos)
        {
            if (id < 128)
            {
                *by++ = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 4;
            }
            else
            {
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 5;
            }
            return by;
        }

        internal unsafe static byte* WriteVLong1(byte* by, long id, ref int pos)
        {
            if (id < 128)
            {
                *by++ = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 4;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L)
            {
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 5;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128)
            {
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 6;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128)
            {
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 7;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 49) | 0x80);
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 8;
            }
            else
            {
                *by++ = (byte)((id >> 56) | 0x80);
                *by++ = (byte)((id >> 49) | 0x80);
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 9;
            }
            return by;
        }

        internal unsafe static byte* WriteVLong1(byte* by, ulong id, ref int pos)
        {
            if (id < 128)
            {
                *by++ = (byte)(id & 0x7F);
                pos++;
            }
            else if (id < 128 * 128)
            {
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 2;
            }
            else if (id < 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 3;
            }
            else if (id < 128 * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 4;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L)
            {
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 5;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128)
            {
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 6;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128)
            {
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 7;
            }
            else if (id < 128 * 128 * 128 * 128 * 128L * 128 * 128 * 128)
            {
                *by++ = (byte)((id >> 49) | 0x80);
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 8;
            }
            else
            {
                *by++ = (byte)((id >> 56) | 0x80);
                *by++ = (byte)((id >> 49) | 0x80);
                *by++ = (byte)((id >> 42) | 0x80);
                *by++ = (byte)((id >> 35) | 0x80);
                *by++ = (byte)((id >> 28) | 0x80);
                *by++ = (byte)((id >> 21) | 0x80);
                *by++ = (byte)((id >> 14) | 0x80);
                *by++ = (byte)((id >> 7) | 0x80);
                *by++ = (byte)(id & 0x7F);
                pos += 9;
            }
            return by;
        }

        #endregion

        #region 值类型字节转换相关

        public unsafe static void BlockCopy(int[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            //fixed (int* pdSrc = &src[srcOffset])
            //{
            //    fixed (byte* pdDst = &dst[dstOffset])
            //    {
            //        int* smem = pdSrc;
            //        int byteCount = count;
            //        byte* dmem = pdDst;
            //        if (byteCount > 0)
            //        {
            //            while (byteCount >= 16)
            //            {
            //                *((uint*)dmem) = *((uint*)smem);
            //                *((uint*)(dmem + 4)) = *((uint*)(smem + 1));
            //                *((uint*)(dmem + 8)) = *((uint*)(smem + 2));
            //                *((uint*)(dmem + 12)) = *((uint*)(smem + 3));
            //                dmem += 16;
            //                smem += 4;
            //                byteCount -= 16;
            //            }
            //            if ((byteCount & 8) != 0)
            //            {
            //                *((uint*)dmem) = *((uint*)smem);
            //                *((uint*)(dmem + 4)) = *((uint*)(smem + 1));
            //                dmem += 8;
            //                smem += 2;
            //            }
            //            if ((byteCount & 4) != 0)
            //            {
            //                *((uint*)dmem) = *((uint*)smem);
            //                dmem += 4;
            //                smem += 1;
            //            }
            //        }
            //    }
            //}


            fixed (int* pdSrc = &src[srcOffset])
            {
                fixed (byte* pdDst = &dst[dstOffset])
                {
                    int* smem = pdSrc;
                    int byteCount = count;
                    int* dmem = (int*)pdDst;
                    if (byteCount > 0)
                    {
                        while (byteCount >= 16)
                        {
                            *(dmem) = *(smem);
                            *((dmem + 1)) = *((smem + 1));
                            *((dmem + 2)) = *((smem + 2));
                            *((dmem + 3)) = *((smem + 3));
                            dmem += 4;
                            smem += 4;
                            byteCount -= 16;
                        }
                        if ((byteCount & 8) != 0)
                        {
                            *(dmem) = *(smem);
                            *((dmem + 1)) = *((smem + 1));
                            dmem += 2;
                            smem += 2;
                        }
                        if ((byteCount & 4) != 0)
                        {
                            *((uint*)dmem) = *((uint*)smem);
                            dmem += 1;
                            smem += 1;
                        }
                    }
                }
            }
        }

        public unsafe static void BlockCopy(byte[] src, int srcOffset, int[] dst, int dstOffset, int count)
        {
            fixed (byte* pdSrc = &src[srcOffset])
            {
                fixed (int* pdDst = &dst[dstOffset])
                {
                    byte* smem = pdSrc;
                    int byteCount = count;
                    int* dmem = pdDst;
                    if (byteCount > 0)
                    {
                        while (byteCount >= 16)
                        {
                            *((uint*)dmem) = *((uint*)smem);
                            *((uint*)(dmem + 1)) = *((uint*)(smem + 4));
                            *((uint*)(dmem + 2)) = *((uint*)(smem + 8));
                            *((uint*)(dmem + 3)) = *((uint*)(smem + 12));
                            dmem += 4;
                            smem += 16;
                            byteCount -= 16;
                        }
                        if ((byteCount & 8) != 0)
                        {
                            *((uint*)dmem) = *((uint*)smem);
                            *((uint*)(dmem + 1)) = *((uint*)(smem + 4));
                            dmem += 2;
                            smem += 8;
                        }
                    }
                }
            }

        }

        public unsafe static void BlockCopy(long[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            fixed (long* pdSrc = &src[srcOffset])
            {
                fixed (byte* pdDst = &dst[dstOffset])
                {
                    uint* smem = (uint*)pdSrc;
                    int byteCount = count;
                    byte* dmem = pdDst;
                    if (byteCount > 0)
                    {
                        //while (byteCount >= 16)
                        //{
                        //    *((uint*)dmem) = *((uint*)smem);
                        //    *((uint*)(dmem + 4)) = *((uint*)(smem + 1));
                        //    *((uint*)(dmem + 8)) = *((uint*)(smem + 2));
                        //    *((uint*)(dmem + 12)) = *((uint*)(smem + 3));
                        //    dmem += 16;
                        //    smem += 4;
                        //    byteCount -= 16;
                        //}
                        //if ((byteCount & 8) != 0)
                        //{
                        //    *((uint*)dmem) = *((uint*)smem);
                        //    *((uint*)(dmem + 4)) = *((uint*)(smem + 1));
                        //    dmem += 8;
                        //    smem += 2;
                        //}

                        while (byteCount >= 16)
                        {
                            *((uint*)dmem) = *(smem);
                            *((uint*)(dmem + 4)) = *(smem + 1);
                            *((uint*)(dmem + 8)) = *(smem + 2);
                            *((uint*)(dmem + 12)) = *(smem + 3);
                            dmem += 16;
                            smem += 4;
                            byteCount -= 16;
                        }
                        if ((byteCount & 8) != 0)
                        {
                            *((uint*)dmem) = *(smem);
                            *((uint*)(dmem + 4)) = *(smem + 1);
                            dmem += 8;
                            smem += 2;
                        }
                    }
                }
            }

        }

        public unsafe static void BlockCopy(byte[] src, int srcOffset, long[] dst, int dstOffset, int count)
        {
            fixed (byte* pdSrc = &src[srcOffset])
            {
                fixed (long* pdDst = &dst[dstOffset])
                {
                    byte* smem = pdSrc;
                    int byteCount = count;
                    long* dmem = pdDst;
                    if (byteCount > 0)
                    {
                        while (byteCount >= 16)
                        {
                            *((uint*)dmem) = *((uint*)smem);
                            *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                            *((uint*)(dmem + 8)) = *((uint*)(smem + 8));
                            *((uint*)(dmem + 12)) = *((uint*)(smem + 12));
                            dmem += 16;
                            smem += 16;
                            byteCount -= 16;
                        }
                        if ((byteCount & 8) != 0)
                        {
                            *((uint*)dmem) = *((uint*)smem);
                            *((uint*)(dmem + 4)) = *((uint*)(smem + 4));
                            dmem += 8;
                            smem += 8;
                        }
                    }
                }
            }

        }

        #endregion
        
        #region test

        unsafe static char* dm = null;
        unsafe static char* sm = null;
        static Action[] act = null;

        private static Action[] GetAct()
        {
            if (act == null)
            {
                act = new Action[65535];
                for (int i = 0; i < 65535; i++)
                {
                    if (i != '"')
                        act[i] = W;
                    else
                        act[i] = WS;
                }
            }
            return act;
        }

        public unsafe static void W()
        {
            *dm++ = *sm++;
        }

        public unsafe static void WS()
        {
            *dm++ = *sm++;
        }

        /// <summary>
        /// 经测试，比switch还慢
        /// </summary>
        /// <param name="dmem"></param>
        /// <param name="smem"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        internal unsafe static int WriteUsAct(char* dmem, char* smem, int charCount)
        {
            dm = dmem;
            sm = smem;
            Action[] act = GetAct();
            for (int i = 0; i < charCount; i++)
                act[*smem]();
            dmem = dm;
            smem = sm;
            return 0;
        }


        #endregion
    }
}
