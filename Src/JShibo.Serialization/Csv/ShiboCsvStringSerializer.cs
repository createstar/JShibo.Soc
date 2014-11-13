using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Reflection;
using System.Text;
using JShibo.Serialization.Common;
using JShibo.Serialization;

namespace JShibo.Serialization.Csv
{
    internal class ShiboCsvStringSerializer : SerializerBase<CsvString, CsvUstring, CsvStringContext, CsvStringSize>
    {

        static ShiboCsvStringSerializer Instance;

        #region 构造函数

        static ShiboCsvStringSerializer()
        {
            Instance = new ShiboCsvStringSerializer();
            Instance.builder = new CsvILBuilder();
            Instance.RegisterAssemblyTypes();
        }

        #endregion

        #region 方法

        internal static void CreateContext(Type type, CsvStringContext info)
        {
            if (Instance.builder.IsBaseType(type) == true)
            {
                info.IsBaseType = true;
                return;
            }

            FieldInfo[] fields = type.GetFields(info.Seting.Flags);
            foreach (FieldInfo field in fields)
            {
                if (Utils.IsIgnoreAttribute(field) == false)
                {
                    string name = Utils.GetAttributeName(field);
                    info.NamesList.Add(name);
                    info.MinSize += name.Length + 3;
                    if (Utils.IsDeep(field.FieldType))
                    {
                        info.SerializeList.Add(Instance.GenerateDataSerializeSurrogate(field.FieldType));
                        info.TypeCountsList.Add(Instance.GetDeserializeTypes(field.FieldType).Length);
                        info.TypesList.Add(field.FieldType);
                        info.NameCountsList.Add(Instance.GetSerializeNames(field.FieldType).Length);
                        CreateContext(field.FieldType, info);
                    }
                    else
                        info.MinSize += Instance.builder.GetSize(field.FieldType);
                }
            }
            PropertyInfo[] propertys = type.GetProperties(info.Seting.Flags);
            foreach (PropertyInfo property in propertys)
            {
                if (Utils.IsIgnoreAttribute(property) == false)
                {
                    string name = Utils.GetAttributeName(property);
                    info.NamesList.Add(name);
                    info.MinSize += name.Length + 3;
                    if (Utils.IsDeep(property.PropertyType))
                    {
                        info.SerializeList.Add(Instance.GenerateDataSerializeSurrogate(property.PropertyType));
                        info.TypeCountsList.Add(Instance.GetDeserializeTypes(property.PropertyType).Length);
                        info.TypesList.Add(property.PropertyType);
                        info.NameCountsList.Add(Instance.GetSerializeNames(property.PropertyType).Length);
                        CreateContext(property.PropertyType, info);
                    }
                    else
                        info.MinSize += Instance.builder.GetSize(property.PropertyType);
                }
            }
        }

        internal static CsvStringContext GetContext(Type type)
        {
            CsvStringContext info = null;
            if (types.TryGetValue(type, out info) == false)
            {
                info = new CsvStringContext();
                CreateContext(type, info);
                info.Serializer = Instance.GenerateDataSerializeSurrogate(type);
                //info.SizeSerialize = GenerateSizeSerializeSurrogate(type);
                //info.Deserialize = GetJsonDeserializeSurrogateFromType(type);
                types.Add(type, info);
                if (info != null)
                    info.ToArray();
            }
            return info; 
        }

        internal static CsvStringContext GetSizeInfos(Type type)
        {
            CsvStringContext info = null;
            if (types.TryGetValue(type, out info) == false)
            {
                info = new CsvStringContext();
                CreateContext(type, info);
                info.SizeSerializer = Instance.GenerateSizeSerializeSurrogate(type);
                //info.SizeSerialize = GenerateSizeSerializeSurrogate(type);
                //info.Deserialize = GetJsonDeserializeSurrogateFromType(type);
                types.Add(type, info);
                if (info != null)
                    info.ToArray();
            }
            return info;
        }


        internal static void LoopSerialize(CsvString stream, object graph)
        {
            CsvString loopStream = new CsvString(stream);
            loopStream.WriteStartObject();

            Type type = graph.GetType();
            CsvStringContext info = null;
            if (type == Instance.lastObjectLoopSerType)
                info = lastObjectLoopTypeInfo;
            else
            {
                Instance.lastObjectLoopSerType = type;
                info = GetContext(type);
                lastObjectLoopTypeInfo = info;
            }
            loopStream.SetInfo(info);

            info.Serializer(loopStream, graph);
            stream.position = loopStream.position;
            stream._buffer = loopStream._buffer;

            if (stream._buffer[stream.position - 1] == ',')
            {
                stream._buffer[stream.position - 1] = '}';
                stream._buffer[stream.position++] = ',';
            }
            else
                stream._buffer[stream.position++] = '}';
        }

        internal static void LoopSerializeList(CsvString stream, IList value)
        {
            CsvString loopStream = new CsvString(stream);
            loopStream.isRoot = false;
            loopStream.WriteStartArray();

            object v = value[0];
            Type type = v.GetType();

            CsvStringContext info = GetContext(type);
            loopStream.SetInfo(info);

            loopStream.WriteStartObject();
            info.Serializer(loopStream, v);
            loopStream.WriteEndObject();

            int count = value.Count;
            for (int i = 1; i < count; i++)
            {
                v = value[i];
                if (v != null)
                {
                    loopStream.current = 0;
                    loopStream.WriteStartObject();
                    info.Serializer(loopStream, value[i]);
                    loopStream.WriteEndObject();
                }
                else
                    loopStream.WriteNullWithoutName();
            }
            stream.position = loopStream.position;
            stream._buffer = loopStream._buffer;
            
            stream.WriteEndArray();
        }

        internal static void LoopSerializeEnumerable(CsvString stream, IEnumerable value)
        {
            IEnumerator it = value.GetEnumerator();
            if (it.MoveNext())
            {
                CsvString loopStream = new CsvString(stream);
                loopStream.isRoot = false;
                loopStream.WriteStartArray();

                object v = it.Current;
                Type type = v.GetType();

                CsvStringContext info = GetContext(type);
                loopStream.SetInfo(info);

                loopStream.WriteStartObject();
                info.Serializer(loopStream, v);
                loopStream.WriteEndObject();

                while (it.MoveNext())
                {
                    v = it.Current;
                    if (v != null)
                    {
                        loopStream.current = 0;
                        loopStream.WriteStartObject();
                        info.Serializer(loopStream, v);
                        loopStream.WriteEndObject();
                    }
                    else
                        loopStream.WriteNullWithoutName();
                }

                stream.position = loopStream.position;
                stream._buffer = loopStream._buffer;

                stream.WriteEndArray();
            }
            else
                stream.WriteZeroArrayWithoutName();
        }

        internal static void LoopSerializeObjectList(CsvString stream, IList value)
        {
            CsvString loopStream = new CsvString(stream);
            loopStream.isRoot = false;
            loopStream.WriteStartArray();

            object v = value[0];
            Type type = v.GetType();
            CsvStringContext info = GetContext(type);
            loopStream.SetInfo(info);

            loopStream.WriteStartObject();
            info.Serializer(loopStream, v);
            loopStream.WriteEndObject();

            int count = value.Count;
            for (int i = 1; i < count; i++)
            {
                v = value[i];
                if (v != null)
                {
                    if (v.GetType() == type)
                    {
                        loopStream.current = 0;
                        loopStream.WriteStartObject();
                        info.Serializer(loopStream, v);
                        loopStream.WriteEndObject();
                    }
                    else
                        LoopSerialize(loopStream, v);
                }
                else
                    loopStream.WriteNullWithoutName();
            }
            stream.position = loopStream.position;
            stream._buffer = loopStream._buffer;

            stream.WriteEndArray();
        }

        #endregion

        #region 公共的

        internal static CsvStringContext GetLastContext(Type type)
        {
            CsvStringContext info = null;
            if (type == Instance.lastSerType)
            {
                info = Instance.lastSerTypeInfo;
            }
            else
            {
                info = GetContext(type);
                Instance.lastSerType = type;
                Instance.lastSerTypeInfo = info;
            }
            return info;
        }

        internal static string Serialize(object graph)
        {
            Type type = graph.GetType();
            CsvStringContext info = GetLastContext(type);
            char[] buffer = CharsBufferManager.GetBuffer(info.MinSize);
            CsvString stream = new CsvString(buffer);
            Serialize(stream, graph, info);
            CharsBufferManager.SetBuffer(stream.GetBuffer());
            return stream.ToString();
        }

        internal static void Serialize(CsvString stream, object graph)
        {
            Type type = graph.GetType();
            CsvStringContext info = GetLastContext(type);
            Serialize(stream, graph, info);
        }

        internal static void Serialize(CsvString stream, object graph, CsvStringContext info)
        {
            if (info.IsBaseType == false)
            {
                stream.SetInfo(info);
                info.Serializer(stream, graph);
                if (stream._buffer[stream.position - 1] == ',')
                    stream.position--;
                if (info.IsBaseType == false)
                {
                    stream._buffer[stream.position++] = '\r';
                    stream._buffer[stream.position++] = '\n';
                }
            }
            else
            {
                stream.isJsonBaseType = true;
                info.Serializer(stream, graph);
                //stream.position -= 2;
            }
        }

        internal static void Serialize(CsvString stream, object graph, Serialize<CsvString> ser)
        {
            stream._buffer[stream.position++] = '{';
            ser(stream, graph);
            if (stream._buffer[stream.position - 1] == ',')
                stream.position--;
            stream._buffer[stream.position++] = '}';
        }

        internal static CsvString SerializeToBuffer(object graph)
        {
            Type type = graph.GetType();
            CsvStringContext info = GetLastContext(type);
            CsvStringSize size = new CsvStringSize();
            info.SizeSerializer(size, graph);
            int totalSize = info.MinSize + size.Size;
            CsvString result = null;
            char[] buffer = null;
            if (totalSize > 400)
            {
                buffer = CharsBufferManager.GetBuffer(totalSize);
                result = new CsvString(buffer);
                Serialize(result, graph, info);
                CharsBufferManager.SetBuffer(result.GetBuffer());
            }
            else
            {
                buffer = new char[totalSize];
                result = new CsvString(buffer);
                Serialize(result, graph, info);
            }
            return result;
        }

        internal static T Deserialize<T>(CsvString stream, CsvStringContext info)
        {
            //if (info.IsJsonBaseType == false)
            //    stream.position++;

            //stream.desers = info.DeserializeStreams;
            //stream.types = info.Types;
            //stream.typeCounts = info.TypeCounts;
            //stream.nameCounts = info.NameCounts;
            //stream.names = info.Names;

            //object value = info.Deserialize(stream);
            //return (T)value;

            return default(T);
        }

        internal static T Deserialize<T>(CsvString stream)
        {
            Type type = typeof(T);
            CsvStringContext info = null;
            if (type == Instance.lastSerType)
            {
                info = Instance.lastSerTypeInfo;
            }
            else
            {
                info = GetContext(type);
                Instance.lastSerType = type;
                Instance.lastSerTypeInfo = info;
            }
            return Deserialize<T>(stream, info);

            //return default(T);
            //throw new Exception("Not supported,it will implementation in next versions");
        }

        #endregion
    }
}
