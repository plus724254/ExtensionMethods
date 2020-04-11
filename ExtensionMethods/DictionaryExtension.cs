using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuashanCRM.Extension
{
    public static class DictionaryExtension
    {
        public static TValue GetValueOrDefaultValue<TKey, TValue> (this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value; 
            if(source.TryGetValue(key, out value))
            {
                return value;
            }
            else 
            {
                return defaultValue;
            }
        }
    }
}