using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using JSONSerializerLib.JSONSerializerLib.Deserializer;
using JSONSerializerLib.JSONSerializerLib.Serializer;

namespace JSONSerializerLib
{
    public class JSONSerializer
    {
        public static string Serialize (object? obj)
        {
            return JSONInnerSerializer.Serialize(obj).Serialize();
        }

        public static T? Deserialize<T>(string str)
        {
            int ind = 0;
            object? obj = JSONInnerDeserializer.Value(ref str, ref ind);
            if (obj is null) return default(T);
            return (T?)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }
    }

    class JsonException : Exception
    {
        public JsonException(string message) : base(message) { }
    }

    namespace JSONSerializerLib.Serializer
    {
        class JSONInnerSerializer
        {
            public static IJsonSerializable Serialize(object? obj)
            {
                if (obj == null) return new SerializableNull();

                Type type = obj.GetType();

                if (type.IsAssignableFrom(typeof(bool))) return new SerializableBool((bool)obj);
                else if (type.IsAssignableFrom(typeof(string))) return new SerializableString((string)obj);
                else if (type.IsAssignableFrom(typeof(sbyte))
                        || type.IsAssignableFrom(typeof(byte))
                        || type.IsAssignableFrom(typeof(ushort))
                        || type.IsAssignableFrom(typeof(short))
                        || type.IsAssignableFrom(typeof(uint))
                        || type.IsAssignableFrom(typeof(int))
                        || type.IsAssignableFrom(typeof(ulong))
                        || type.IsAssignableFrom(typeof(long))
                        || type.IsAssignableFrom(typeof(char))
                        || type.IsAssignableFrom(typeof(float))
                        || type.IsAssignableFrom(typeof(double))
                        || type.IsAssignableFrom(typeof(decimal))) return new SerializableNumeric((double)Convert.ChangeType(obj, typeof(double)));
                else if (typeof(IDictionary).IsAssignableFrom(type)) return new SerializableDict(obj);
                else if (typeof(ICollection).IsAssignableFrom(type)) return new SerializableArray(obj);
                else return new SerializableObject(obj);
            }
        }

        interface IJsonSerializable
        {
            public string Serialize();
        }

        class SerializableNumeric : IJsonSerializable
        {
            private double num;

            public SerializableNumeric(double num)
            {
                this.num = num;
            }

            public string Serialize()
            {
                NumberFormatInfo ti = new();
                ti.NumberDecimalSeparator = ".";
                return num.ToString(ti);
            }
        }

        class SerializableString : IJsonSerializable
        {
            private string str;

            public SerializableString(string str)
            {
                this.str = str;
            }

            public string Serialize()
            {
                return $"\"{str}\"";
            }
        }

        class SerializableBool : IJsonSerializable
        {
            private bool val;

            public SerializableBool(bool val)
            {
                this.val = val;
            }

            public string Serialize()
            {
                return val ? "true" : "false";
            }
        }

        class SerializableArray : IJsonSerializable
        {
            private List<IJsonSerializable> array = new List<IJsonSerializable>();

            public SerializableArray(object obj)
            {
                ICollection collection = (ICollection)obj;
                Type obj_type = obj.GetType();
                Type[] type_parameters = obj_type.GetGenericArguments();
                string gen_type_str = type_parameters[0].FullName;
                string gen_assemblyname_str = type_parameters[0].Assembly.FullName;

                array.Add(new SerializableString(gen_type_str));
                array.Add(new SerializableString(gen_assemblyname_str));

                foreach (var item in collection)
                {
                    array.Add(JSONInnerSerializer.Serialize(item));
                }
            }

            public string Serialize()
            {
                string ser_str = string.Join(", ", array.Select((ser) => ser.Serialize()));
                return $"[{ser_str}]";
            }
        }

        class SerializableDict : IJsonSerializable
        {
            protected Dictionary<string, IJsonSerializable> dict = new Dictionary<string, IJsonSerializable>();

            public SerializableDict(object obj)
            {
                IDictionary dictionary = (IDictionary)obj;

                Type obj_type = obj.GetType();
                Type[] type_parameters = obj_type.GetGenericArguments();
                string dicttype_str = type_parameters[1].FullName;
                string assemblyname_str = type_parameters[1].Assembly.FullName;

                dict["_dicttype"] = new SerializableString(dicttype_str);
                dict["_assemblyname"] = new SerializableString(assemblyname_str);

                foreach (DictionaryEntry item in dictionary) 
                {
                    dict[$"{(string)item.Key}"] = JSONInnerSerializer.Serialize(item.Value);
                }
            }

            public string Serialize()
            {
                string ser_str = string.Join(", ", dict.Select((kvp) => $"\"{kvp.Key}\"" + ": " + kvp.Value.Serialize()));
                return $"{{ {ser_str} }}";
            }
        }

        class SerializableObject : SerializableDict
        {
            public SerializableObject(object obj) : base(ObjToDict(obj)) 
            {
                dict.Remove("_dicttype");
                Type objType = obj.GetType();
                dict["_classname"] = new SerializableString(objType.FullName);
                dict["_assemblyname"] = new SerializableString(objType.Assembly.FullName);
            }

            private static IDictionary ObjToDict(object obj)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                Type objType = obj.GetType();

                FieldInfo[] fieldInfos = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    dict[fieldInfo.Name] = fieldInfo.GetValue(obj);
                }

                return dict;
            }
        }

        class SerializableNull : IJsonSerializable
        {
            public SerializableNull() { }

            public string Serialize() { return "null"; }
        }
    }

    namespace JSONSerializerLib.Deserializer
    {
        class JSONInnerDeserializer
        {
            public static object? Value(ref string json_str, ref int ind)
            {
                for (; ind < json_str.Length; ind++)
                {
                    if (json_str[ind] == 'n') { return Null(ref json_str, ref ind); }
                    else if (json_str[ind] == 't' || json_str[ind] == 'f') { return Bool(ref json_str, ref ind); }
                    else if (json_str[ind] == '"') { return String(ref json_str, ref ind); }
                    else if (char.IsDigit(json_str[ind]) || json_str[ind] == '-') { return Num(ref json_str, ref ind); }
                    else if (json_str[ind] == '[') { return Array(ref json_str, ref ind); }
                    else if (json_str[ind] == '{') { return DictOrObj(ref json_str, ref ind); }
                }

                throw new JsonException("No value found");
            }

            private static object DictOrObj(ref string json_str, ref int ind)
            {
                Dictionary<string, object> raw_dict = RawDict(ref json_str, ref ind);

                if (raw_dict.ContainsKey("_dicttype") && raw_dict.ContainsKey("_assemblyname"))
                {
                    string dicttype_str = (string)raw_dict["_dicttype"];
                    string assemblyname_str = (string)raw_dict["_assemblyname"];
                    Type? dict_type = Type.GetType(dicttype_str + ", " + assemblyname_str);

                    if (dict_type is null)
                    {
                        throw new JsonException("Invalid dictionary type");
                    }

                    Type[] dict_types = { typeof(string), dict_type };
                    Type dict_definition_type = typeof(Dictionary<,>);
                    Type res_dict_type = dict_definition_type.MakeGenericType(dict_types);

                    object? obj = Activator.CreateInstance(res_dict_type);

                    if (obj is null)
                    {
                        throw new JsonException("Failed to construct dict");
                    }

                    IDictionary dict_obj = (IDictionary)obj;

                    raw_dict.Remove("_dicttype");
                    raw_dict.Remove("_assemblyname");

                    foreach (KeyValuePair<string, object> kvp in raw_dict)
                    {
                        dict_obj.Add(kvp.Key, Convert.ChangeType(kvp.Value, dict_type));
                    }

                    return dict_obj;
                }
                else if (raw_dict.ContainsKey("_classname") && raw_dict.ContainsKey("_assemblyname"))
                {
                    string classname_str = (string)raw_dict["_classname"];
                    string assemblyname_str = (string)raw_dict["_assemblyname"];
                    Type? class_type = Type.GetType(classname_str + ", " + assemblyname_str);

                    if (class_type is null)
                    {
                        throw new JsonException("Invalid class type");
                    }

                    object? obj = Activator.CreateInstance(class_type);

                    if (obj is null)
                    {
                        throw new JsonException("Failed to construct class");
                    }

                    raw_dict.Remove("_classname");
                    raw_dict.Remove("_assemblyname");

                    foreach (KeyValuePair<string, object> kvp in raw_dict)
                    {
                        var field = class_type.GetField(kvp.Key);
                        if (field is null)
                        {
                            throw new JsonException("field_type is null");
                        }
                        var field_type = field.FieldType;

                        field.SetValue(obj, Convert.ChangeType(kvp.Value, field_type));
                    }

                    return obj;
                }
                else
                {
                    throw new JsonException("Invalid dictionary type");
                }
            }

            private static Dictionary<string, object> RawDict(ref string json_str, ref int ind)
            {
                string dict_block = ReadDictBlock(ref json_str, ref ind);
                int dict_block_ind = 0;

                Dictionary<string, object> dict = new Dictionary<string, object>();

                while (dict_block_ind < dict_block.Length)
                {
                    Tuple<string, object> kvp = Pair(ref dict_block, ref dict_block_ind);
                    dict.Add(kvp.Item1, kvp.Item2);
                }

                return dict;
            }

            private static object Array(ref string json_str, ref int ind)
            {
                List<object?> list = RawArray(ref json_str, ref ind);

                if (list.Count < 2)
                {
                    throw new JsonException("Invalid list format");
                }

                string array_type_str = (string)(list[0] ?? throw new JsonException("Invalid type name"));
                string array_assembly_str = (string)(list[1] ?? throw new JsonException("Invalid type name"));
                list.RemoveRange(0, 2);
                Type? array_type = Type.GetType(array_type_str + ", " + array_assembly_str);

                if (array_type == null)
                {
                    throw new JsonException("Invalide list type");
                }

                Type[] array_types = { array_type };
                Type list_definition_type = typeof(List<>);
                Type list_type = list_definition_type.MakeGenericType(array_types);

                object? obj = Activator.CreateInstance(list_type);

                if (obj is null)
                {
                    throw new JsonException("Failed to construct list");
                }

                IList list_obj = (IList)obj;

                foreach (object? elem in list)
                {
                    list_obj.Add(Convert.ChangeType(elem, array_type, CultureInfo.InvariantCulture));
                }

                return list_obj;
            }

            private static List<object?> RawArray(ref string json_str, ref int ind)
            {
                string array_block = ReadArrayBlock(ref json_str, ref ind);
                int array_block_ind = 0;

                List<object?> list = new List<object?>();

                while (array_block_ind < array_block.Length)
                {
                    list.Add(Value(ref array_block, ref array_block_ind));
                }

                return list;
            }

            private static Tuple<string, object> Pair(ref string json_str, ref int ind)
            {
                string name = Name(ref json_str, ref ind);
                object obj = Value(ref json_str, ref ind);

                return Tuple.Create(name, obj);
            }

            private static string Name(ref string json_str, ref int ind)
            {
                string str = ReadToPairDelim(ref json_str, ref ind);

                return StringEntity(str);
            }

            private static object Num(ref string json_str, ref int ind)
            {
                string str = ReadToDelim(ref json_str, ref ind);
                double res_num;

                NumberFormatInfo ti = new();
                ti.NumberDecimalSeparator = ".";

                if (double.TryParse(str, NumberStyles.Float, ti, out res_num))
                {
                    return res_num;
                }
                else
                {
                    throw new JsonException("Invalid numeric format");
                }
            }

            private static object Bool(ref string json_str, ref int ind)
            {
                string str = ReadToDelim(ref json_str, ref ind);
                bool res_bool;

                res_bool = str == "true" ? true :
                    str == "false" ? false : throw new JsonException("Invalid bool format");

                return res_bool;
            }

            private static object String(ref string json_str, ref int ind)
            {
                string str = ReadToDelim(ref json_str, ref ind);

                return StringEntity(str);
            }

            private static object? Null(ref string json_str, ref int ind)
            {
                string str = ReadToDelim(ref json_str, ref ind);

                if (str == "null")
                {
                    return null;
                }
                else
                {
                    throw new JsonException("Invalid null format");
                }
            }

            private static string StringEntity(string str)
            {
                if (str.Length >= 2)
                {
                    if (str[0] == '\"' && str[str.Length - 1] == '\"')
                    {
                        return str.Substring(1, str.Length - 2);
                    }
                    else
                    {
                        throw new JsonException("Invalid string format");
                    }
                }
                else
                {
                    throw new JsonException("Invalid string format");
                }
            }

            private static string ReadToDelim(ref string json_str, ref int ind)
            {
                string res_str = "";
                bool in_quotes = false;

                for (; ind < json_str.Length && (json_str[ind] != ',' || in_quotes); ind++)
                {
                    if (json_str[ind] == '\"') in_quotes ^= true;
                    res_str += json_str[ind];
                }

                if (ind < json_str.Length) ind++;

                return res_str.Trim();
            }

            private static string ReadToPairDelim(ref string json_str, ref int ind)
            {
                string res_str = "";
                bool in_quotes = false;

                for (; ind < json_str.Length && (json_str[ind] != ':' || in_quotes); ind++)
                {
                    if (json_str[ind] == '\"') in_quotes ^= true;
                    res_str += json_str[ind];
                }

                if (ind < json_str.Length) ind++;

                return res_str.Trim();
            }

            private static string ReadArrayBlock (ref string json_str, ref int ind)
            {
                string res_str = "";
                bool in_quotes = false;
                int skip = 0;

                if (json_str.Length < 2 || json_str[ind] != '[')
                {
                    throw new JsonException("Not found array block");
                }

                for (; ind < json_str.Length; ind++)
                {
                    if (json_str[ind] == '\"') in_quotes ^= true;
                    if (json_str[ind] == '[') skip++;
                    if (json_str[ind] == ']') skip--;
                    res_str += json_str[ind];

                    if (json_str[ind] == ']' && !in_quotes && skip == 0) break;
                }

                if (res_str[res_str.Length - 1] != ']')
                {
                    throw new JsonException("Not found array block");
                }

                for (; ind < json_str.Length && json_str[ind] != ','; ind++) { }
                ind++;

                return res_str.Substring(1, res_str.Length - 2);
            }

            private static string ReadDictBlock(ref string json_str, ref int ind)
            {
                string res_str = "";
                bool in_quotes = false;
                int skip = 0;

                if (json_str.Length < 2 || json_str[ind] != '{')
                {
                    throw new JsonException("Not found dict block");
                }

                for (; ind < json_str.Length; ind++)
                {
                    if (json_str[ind] == '\"') in_quotes ^= true;
                    if (json_str[ind] == '{') skip++;
                    if (json_str[ind] == '}') skip--;
                    res_str += json_str[ind];

                    if (json_str[ind] == '}' && !in_quotes && skip == 0) break;
                }

                if (res_str[res_str.Length - 1] != '}')
                {
                    throw new JsonException("Not found dict block");
                }

                for (; ind < json_str.Length && json_str[ind] != ','; ind++) { }
                ind++;

                return res_str.Substring(1, res_str.Length - 2);
            }
        }
    }
}