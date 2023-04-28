using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StaticDictionary.Interface
{

    public interface IStaticDictionaryFactory<TKey, TValue> where TKey : notnull
    {

        public static IReadOnlyDictionary<TKey, TValue> CreateStaticDictionary()
        {
            return null;
        }
    }

    public interface IStaticDictionaryFactoryDefinition<TKey, TValue> : IStaticDictionaryFactory<TKey,TValue> where TKey: notnull
    {
        static TKey[] Keys;
        static TValue[] Values;
    }

}
