using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace StaticDictionary.Generator
{
    internal struct DictionaryFactoryData
    {
		/// <summary>
		/// capture the access modifier (public private protected internal).
		/// before the string "partial class".
		/// capure the name of the class .
		/// which extends some version of "IStaticDictionaryFactoryDefinition".
        /// capture the key type and the value type.
        /// to capture the keys search for "static {keytype}[] Keys = " and capture everything to the first instance of "};".
        /// use the same technique to capture the values.
		/// </summary>
		public static readonly Regex DictExpression = new Regex(@"(?<Access>\S+)\s+(?>partial\s+class)\s+(?<Name>\S+)\s*:\s*(?>IStaticDictionaryFactoryDefinition<(?<KeyType>\S+)\s*,\s*(?<ValueType>\S+)\s*>)[\S\s]+?\s(?>static\s+\k<KeyType>\[\]\s+Keys\s+=\s+{(?<Keys>[\S\s]+?)};)[\S\s]+?\s(?>static\s+\k<ValueType>\[\]\s+Values\s+=\s+{(?<Values>[\S\s]+?)};)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        private static string[] parseArray(string value)
        {
            List<string> res = new List<string>();
            StringBuilder CurrentValue = new StringBuilder();
            int level = 0;
            char prev = '\0';
            bool IsInString = false;
            foreach (char c in value)
            {
				if (level > 0 || c != ',')
				{
					CurrentValue.Append(c);
				}
				switch (c)
                {
                    case '"':
                        if(IsInString)
                        {
                            if(prev != '\\')
                            {
                                IsInString = false;
                                --level;
                            }
                        }
                        else
                        {
                            ++level;
                            IsInString = true;
                        }
                        break;
                    case '(':
                    case '{':
                        ++level;     
                        break;
                    case ')':
                    case '}':
                        --level;
                        break;
                    case ',':
                        if(level == 0)
                        {
                            res.Add(CurrentValue.ToString().Trim());
                            CurrentValue.Clear();
                        }
                        break;
                }
            }
            res.Add(CurrentValue.ToString().Trim());
            return res.ToArray();
        }

        public string[] KeysArray;
        public string[] ValuesArray;

        public string KeyType;
        public string ValueType;

        public string ClassName;
        public string AccessModifier;
        public string NameSpace;

        public DictionaryFactoryData(string nameSpace, Match DictionaryMatch)
        {
            NameSpace = nameSpace;

            AccessModifier = DictionaryMatch.Groups["Access"].Value.Trim();

            ClassName = DictionaryMatch.Groups["Name"].Value.Trim();

            KeyType = DictionaryMatch.Groups["KeyType"].Value.Trim();
            ValueType = DictionaryMatch.Groups["ValueType"].Value.Trim();

            string Keys = DictionaryMatch.Groups["Keys"].Value.Trim();
            string Values = DictionaryMatch.Groups["Values"].Value.Trim();

            KeysArray = parseArray(Keys);
            ValuesArray = parseArray(Values);
        }

        private struct ParsedData
        {
            public string Namespace;
            public Match Data;
            private static IEnumerable<ParsedData> ParseSnippet(string Namespace, string snippet)
            {
                foreach(Match m in DictExpression.Matches(snippet))
                {
                    yield return new ParsedData { Namespace = Namespace, Data = m };
                }
            }
			internal static IEnumerable<ParsedData> ParseFile(string fileContent)
			{
                string NameSpace = "";
                int SnippetStart = 0;
                int snippetEnd = 0;
                while(snippetEnd < fileContent.Length - 1)
                {
                    snippetEnd = fileContent.IndexOf("namespace", SnippetStart, StringComparison.Ordinal);
                    if(snippetEnd == -1)
                    {
                        snippetEnd = fileContent.Length - 1;
                    }
                    foreach(var data in ParseSnippet(NameSpace, fileContent.Substring(SnippetStart, snippetEnd - SnippetStart))) 
                    {
                        yield return data;
                    }
                    int NamespaceStart = snippetEnd + 10;
                    if (NamespaceStart > fileContent.Length - 1) break;
                    while (char.IsWhiteSpace(fileContent[NamespaceStart])) ++NamespaceStart;
                    int NameSpaceLength = 1;
                    while (!char.IsWhiteSpace(fileContent[NamespaceStart + NameSpaceLength])) ++NameSpaceLength;

                    NameSpace = fileContent.Substring(NamespaceStart, NameSpaceLength);
                    SnippetStart = NamespaceStart + NameSpaceLength + 1;
                }
			}
		}

        public static IEnumerable<DictionaryFactoryData> ParseFile(string FileContent)
        {
            foreach(var data in ParsedData.ParseFile(FileContent))
            {
                yield return new DictionaryFactoryData(data.Namespace, data.Data);
            }
        }
        

    }
}
