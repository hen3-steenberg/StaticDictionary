using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace StaticDictionary.Generator
{
	internal class DictionaryFactory
	{
		public DictionaryFactory(DictionaryFactoryData _data)
		{
			data = _data;
		}

		private readonly DictionaryFactoryData data;

		private int Count => Math.Min(data.KeysArray.Length, data.ValuesArray.Length);

		private string GetKeyType()
		{
			return data.KeyType;
		}

		private string GetValueType()
		{
			return data.ValueType;
		}

		private static bool IsBuiltin(string typename)
		{
			switch(typename)
			{
				case "string":
				case "String":
				case "System.String":
				case "bool":
				case "Boolean":
				case "System.Boolean":
				case "byte":
				case "Byte":
				case "System.Byte":
				case "sbyte":
				case "SByte":
				case "System.SByte":
				case "char":
				case "Char":
				case "System.Char":
				case "decimal":
				case "Decimal":
				case "System.Decimal":
				case "double":
				case "Double":
				case "System.Double":
				case "float":
				case "Single":
				case "System.Single":
				case "int":
				case "Int32":
				case "System.Int32":
				case "uint":
				case "UInt32":
				case "System.UInt32":
				case "nint":
				case "IntPtr":
				case "System.IntPtr":
				case "nuint":
				case "UIntPtr":
				case "System.UIntPtr":
				case "long":
				case "Int64":
				case "System.Int64":
				case "ulong":
				case "UInt64":
				case "System.UInt64":
				case "short":
				case "Int16":
				case "System.Int16":
				case "ushort":
				case "UInt16":
				case "System.UInt16":
					return true;
				default: 
					return false;
			}
		}

		private static bool IsEnum(string typename)
		{
			//TODO : Check if the provided type is an enum, resume false in the interim
			return false;
		}

		private string GetValueModifier()
		{
			
			if(IsBuiltin(GetValueType()) || IsEnum(GetValueType()))
			{
				return "const";
			}
			else
			{
				return "static readonly";
			}
		}

		private string getDictClassName()
		{
			return $"{data.ClassName}_Dictionary";
		}

		private string getDictClassHeader()
		{
			return $"{data.AccessModifier} class {getDictClassName()} : IReadOnlyDictionary<{GetKeyType()},{GetValueType()}>";
		}

		private string getValueDefinitions()
		{
			StringBuilder valueBuilder = new StringBuilder();
			valueBuilder.AppendLine("#region ValueDefinitions");
			valueBuilder.AppendLine();
			for (int index = 0; index < Count; ++index)
			{
				valueBuilder.AppendLine($"\t\tprivate {GetValueModifier()} {GetValueType()} val_{index} = {data.ValuesArray[index]};");
				valueBuilder.AppendLine();
			}
			valueBuilder.AppendLine("\t\t#endregion");

			return valueBuilder.ToString();
		}

		private string getKeyValueDefinitions()
		{
			StringBuilder kvBuilder = new StringBuilder();
			kvBuilder.AppendLine("#region KeyValueDefinitions");
			kvBuilder.AppendLine();
			for (int index = 0; index < Count; ++index)
			{
				kvBuilder.AppendLine($"\t\tprivate static readonly KeyValuePair<{GetKeyType()},{GetValueType()}> kv_{index} = new KeyValuePair<{GetKeyType()},{GetValueType()}>({data.KeysArray[index]}, val_{index});");
				kvBuilder.AppendLine();
			}
			kvBuilder.AppendLine("\t\t#endregion");

			return kvBuilder.ToString();

		}

		private string getGetImpl()
		{
			StringBuilder IndexBuilder = new StringBuilder();

			IndexBuilder.AppendLine($"private static {GetValueType()} GetImpl({GetKeyType()} key)");
			IndexBuilder.AppendLine("\t\t{");

			IndexBuilder.AppendLine("\t\t\tswitch(key)");
			IndexBuilder.AppendLine("\t\t\t{");

			for (int index = 0; index < Count; ++index)
			{
				IndexBuilder.AppendLine($"\t\t\t\tcase {data.KeysArray[index]}:");
				IndexBuilder.AppendLine($"\t\t\t\t\treturn val_{index};");
			}

			IndexBuilder.AppendLine("\t\t\t\tdefault:");
			IndexBuilder.AppendLine($"\t\t\t\t\treturn default({GetValueType()});");

			IndexBuilder.AppendLine("\t\t\t}");

			IndexBuilder.AppendLine("\t\t}");

			return IndexBuilder.ToString();
		}

		private string getGet()
		{
			return $@"public static {GetValueType()} GetValue({GetKeyType()} key)
		{{
			{GetValueType()} res = GetImpl(key);
			if(res is null)
				throw new ArgumentOutOfRangeException(""key"");
			else
				return res;
		}}";
		}

		private string getKeyEnumerator()
		{
			StringBuilder KeyBuilder = new StringBuilder();
			KeyBuilder.AppendLine($"public static IEnumerable<{GetKeyType()}> GetKeys()");
			KeyBuilder.AppendLine("\t\t{");

			for (int index = 0; index < Count; ++index)
			{
				KeyBuilder.AppendLine($"\t\t\tyield return {data.KeysArray[index]};");
			}

			KeyBuilder.AppendLine("\t\t}");
			return KeyBuilder.ToString();
		}

		private string getValueEnumerator()
		{
			StringBuilder ValueBuilder = new StringBuilder();
			ValueBuilder.AppendLine($"public IEnumerable<{GetValueType()}> GetValues()");
			ValueBuilder.AppendLine("\t\t{");

			for (int index = 0; index < Count; ++index)
			{
				ValueBuilder.AppendLine($"\t\t\tyield return val_{index};");
			}

			ValueBuilder.AppendLine("\t\t}");
			return ValueBuilder.ToString();
		}

		private string getCount()
		{
			return $"public static int Size()\r\n\t\t{{\r\n\t\t\treturn {Count};\r\n\t\t}}";
		}

		private string getContains()
		{
			StringBuilder ContainsBuilder = new StringBuilder();

			ContainsBuilder.AppendLine($"public static bool ContainsKey({GetKeyType()} key)");
			ContainsBuilder.AppendLine("\t\t{");

			ContainsBuilder.AppendLine("\t\t\tswitch(key)");
			ContainsBuilder.AppendLine("\t\t\t{");

			for (int index = 0; index < Count; ++index)
			{
				ContainsBuilder.AppendLine($"\t\t\t\tcase {data.KeysArray[index]}:");
			}
			ContainsBuilder.AppendLine("\t\t\t\t\treturn true;");

			ContainsBuilder.AppendLine("\t\t\t\tdefault:");
			ContainsBuilder.AppendLine("\t\t\t\t\treturn false;");

			ContainsBuilder.AppendLine("\t\t\t}");

			ContainsBuilder.AppendLine("\t\t}");

			return ContainsBuilder.ToString();
		}

		private string getEnumerator()
		{
			StringBuilder EnumeratorBuilder = new StringBuilder();
			EnumeratorBuilder.AppendLine($"public static IEnumerable<KeyValuePair<{GetKeyType()}, {GetValueType()}>> Enumerate()");
			EnumeratorBuilder.AppendLine("\t\t{");

			for (int index = 0; index < Count; ++index)
			{
				EnumeratorBuilder.AppendLine($"\t\t\tyield return kv_{index};");
			}

			EnumeratorBuilder.AppendLine("\t\t}");
			return EnumeratorBuilder.ToString();
		}

		private string getTryGet()
		{
			return $@"public static bool TryGetValue({GetKeyType()} key, [MaybeNullWhen(false)] out {GetValueType()} value)
        {{
            value = GetImpl(key);
			return !(value is null);
        }}";
		}

		private string getImplClassHeader()
		{
			return $"{data.AccessModifier} partial class {data.ClassName} : IStaticDictionaryFactory<{GetKeyType()},{GetValueType()}>";
		}

		private string getReadOnlyDictionaryImpl()
		{
			string key = GetKeyType();
			string value = GetValueType();
			string kv = $"<{key}, {value}>";
			var str = $@"bool IReadOnlyDictionary{kv}.ContainsKey({key} key)
		{{
			return ContainsKey(key);
		}}

		bool IReadOnlyDictionary{kv}.TryGetValue({key} key, [MaybeNullWhen(false)] out {value} value)
		{{
			return TryGetValue(key, out value);
		}}

		public IEnumerator<KeyValuePair{kv}> GetEnumerator()
		{{
			return Enumerate().GetEnumerator();
		}}

		IEnumerator IEnumerable.GetEnumerator()
		{{
			return Enumerate().GetEnumerator();
		}}

		public IEnumerable<{key}> Keys => GetKeys();

		public IEnumerable<{value}> Values => GetValues();

		public int Count => Size();

		public {value} this[{key} key] => GetImpl(key);";

			return str;
		}

		public override string ToString()
		{
			string res = $@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StaticDictionary.Interface;

namespace {data.NameSpace}
{{
    {getDictClassHeader()}
    {{
        {getValueDefinitions()}

		{getKeyValueDefinitions()}

        {getGetImpl()}

		{getGet()}

        {getKeyEnumerator()}

        {getValueEnumerator()}

        {getCount()}

        {getContains()}

        {getEnumerator()}

        {getTryGet()}

		{getReadOnlyDictionaryImpl()}

    }}

    {getImplClassHeader()}
    {{
        public static IReadOnlyDictionary<{GetKeyType()}, {GetValueType()}> CreateStaticDictionary()
        {{
            return new {getDictClassName()}();
        }}
    }}
}}";
			return res;
		}

		public GeneratorSource GetSource()
		{
			return new GeneratorSource
			{
				FileName = $"{data.ClassName}.g.cs",
				Source = ToString()
			};
		}

		public static IEnumerable<GeneratorSource> ParseFile(string FileContent)
		{
			foreach(var dictData in DictionaryFactoryData.ParseFile(FileContent))
			{
				DictionaryFactory factory = new DictionaryFactory(dictData);
				yield return factory.GetSource();
			}
		}

	}
}
