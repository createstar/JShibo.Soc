using JShibo.Serialization;
using JShibo.Serialization.Json;
using JShibo.Serialization.Soc;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using JShibo.Serialization.Csv;
using System.IO;

namespace JShibo.Serialization
{

    public class ObjectBufferContext : ObjectContext<ObjectBuffer,ObjectUbuffer, ObjectBufferSize>
    {
        public byte[] Serialize(object graph)
        {
            ObjectBuffer buffer = new ObjectBuffer();
            Serializer(buffer, graph);
            return buffer.ToArray();
        }

        internal void Serialize(Stream stream, object graph)
        {
            int size = ShiboObjectBufferSerializer.GetSize(this, graph);
            ObjectBuffer buffer = new ObjectBuffer(size);
            Serializer(buffer, graph);
            stream.Write(buffer.GetBuffer(), 0, buffer.position);
        }

        public void Serialize(ObjectBuffer stream, object graph)
        {
            Serializer(stream, graph);
        }

        public T Deserialize<T>(ObjectUbuffer stream)
        {
            //if (info.IsJsonBaseType == false)
            //    stream.position++;

            stream.desers = this.Deserializers;
            stream.types = this.Types;
            stream.typeCounts = this.TypeCounts;
            //stream.nameCounts = info.NameCounts;
            //stream.names = info.Names;
            //stream.nameLens = info.NameLens;

            object value =Deserializer(stream);
            return (T)value;
        }

    }

    public class ObjectStreamContext : ObjectContext<ObjectStream, ObjectUstream, ObjectStreamSize>
    {


    }

    public class JsonStringContext : JsonContext<JsonString, JsonUstring, JsonStringSize>
    {
    }

    public class CsvStringContext : CsvContext<CsvString, CsvUstring, CsvStringSize>
    {
    }

    public class XType<Data, Info, Size>
    {
        #region old
        //internal static Dictionary<Type, Type[]> deTypes;
        //internal static Dictionary<Type, Serialize<Data>> jsonTypeMap;
        //internal static Dictionary<Type, Serialize<Size>> jsonTypeSizeMap;
        //internal static Dictionary<Type, Deserialize<Data>> jsonDeTypeMap;
        //internal static Dictionary<Type, Info> jsonTypes;
        //internal static Dictionary<Type, string[]> namesMap;
        #endregion

        internal Type[] deTypes;
        internal Serialize<Data> jsonTypeMap;
        internal Serialize<Size> jsonTypeSizeMap;
        internal Deserialize<Data> jsonDeTypeMap;
        internal Info jsonTypes;
        internal string[] namesMap;

        public XType()
        { 
        
        }
    }

    public class ObjectContext<Data, UData, Size>
    {
        internal SerializerSettings Seting = SerializerSettings.Default;
        internal Serialize<Data> Serializer;
        internal Serialize<Size> SizeSerializer;
        internal Deserialize<UData> Deserializer;
        
        internal int ObjectCount = 0;
        internal bool IsBaseType = false;
        internal bool IsAllBaseType = true;
        
        internal List<Serialize<Data>> SerializeList;
        internal List<Serialize<Size>> SizeSerializeList;
        internal List<Deserialize<UData>> DeserializeList;
        internal List<Type> TypesList;
        internal List<int> TypeCountsList;
        
        internal Serialize<Data>[] Serializers;
        internal Serialize<Size>[] SizeSerializers;
        internal Deserialize<UData>[] Deserializers;
        internal Type[] Types;
        internal int[] TypeCounts;
        internal int MinSize = 0;
        /// <summary>
        /// 定义的容量大小
        /// </summary>
        internal int DefineSize = 16;

        internal ObjectContext()
        {
            SerializeList = new List<Serialize<Data>>();
            SizeSerializeList = new List<Serialize<Size>>();
            DeserializeList = new List<Deserialize<UData>>();
            TypesList = new List<Type>();
            TypeCountsList = new List<int>();
            
        }

        internal virtual void ToArray()
        {
            Serializers = SerializeList.ToArray();
            SizeSerializers = SizeSerializeList.ToArray();
            Deserializers = DeserializeList.ToArray();
            Types = TypesList.ToArray();
            TypeCounts = TypeCountsList.ToArray();
        }

    }

    public class ObjectInitializeContext<UData>
    {
        internal SerializerSettings Seting = SerializerSettings.Default;
        internal Deserialize<UData> Deserializer;
        internal List<Deserialize<UData>> DesList;
        internal Deserialize<UData>[] Deserializes;
        /// <summary>
        /// 定义的容量大小
        /// </summary>
        internal int DefineSize = 16;

        internal ObjectInitializeContext()
        {
            DesList = new List<Deserialize<UData>>();
        }

        internal virtual void ToArray()
        {
            Deserializes = DesList.ToArray();
        }

    }

    public class TextContext<Data, UData, Size> : ObjectContext<Data, UData, Size>
    {
        internal List<string> NamesList;
        internal List<CheckAttribute> ChecksList;
        internal List<int> NameCountsList;

        internal string[] Names;
        internal CheckAttribute[] checks;
        internal int[] NameCounts;
        private string[] NamesCamelCase;
        /// <summary>
        /// 是否存在大小写名称相同的情况
        /// </summary>
        internal bool IsHaveUpperLowerSame = false;

        internal TextContext()
        {
            NamesList = new List<string>();
            ChecksList = new List<CheckAttribute>();
            NameCountsList = new List<int>();

            MinSize = 32;
        }

        internal override void ToArray()
        {
            base.ToArray();
            Names = NamesList.ToArray();
            checks = ChecksList.ToArray();
            NameCounts = NameCountsList.ToArray();
        }

        internal string[] GetNamesCamelCase()
        {
            if (NamesCamelCase == null)
            {
                List<string> namesCamelCaseList = new List<string>(NamesList.Count);
                foreach (string name in NamesList)
                {
                    if (name[0] > 96 && name[0] < 123)
                    {
                        char[] chs = name.ToCharArray();
                        chs[0] = (char)(chs[0] - 32);
                        namesCamelCaseList.Add(new string(chs));
                    }
                    else
                        namesCamelCaseList.Add(name);
                }
                NamesCamelCase = namesCamelCaseList.ToArray();
            }
            return NamesCamelCase;
        }
    }

    public class JsonContext<Data, UData, Size> : TextContext<Data, UData, Size>
    {
        internal int[] NameLens;
        internal byte[] NamesBlock;

        internal void CreateBlock()
        {
            List<int> nameLensList = new List<int>();
            int totalLen = 0, cur = 0;
            foreach (string name in NamesList)
                totalLen += Encoding.UTF8.GetByteCount(name);
            NamesBlock = new byte[totalLen];
            foreach (string name in NamesList)
            {
                int len = Encoding.UTF8.GetBytes(name, 0, name.Length, NamesBlock, cur);
                cur += len;
                nameLensList.Add(len);
            }
            NameLens = nameLensList.ToArray();
        }
    }

    public class CsvContext<Data, UData, Size> : TextContext<Data, UData, Size>
    {
        
    }
    
}
