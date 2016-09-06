using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Baseline;
using Marten.Util;
using Npgsql;

namespace Marten.Linq.Compiled
{
    internal interface IDbParameterSetter
    {
        NpgsqlParameter AddParameter(object query, NpgsqlCommand command);
    }

    public class ContainmentParameterSetter : IDbParameterSetter
    {


        public NpgsqlParameter AddParameter(object query, NpgsqlCommand command)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DictionaryElement<T, TElement>
    {
        public string[] Keys { get; }
        private readonly Func<T, TElement> _getter;

        public DictionaryElement(EnumStorage storage, string[] keys, MemberInfo member)
        {
            Keys = keys;
            _getter = LambdaBuilder.Getter<T, TElement>(storage, new []{member});

            Member = member;
        }

        public MemberInfo Member { get; }

        public void Write(T target, IDictionary<string, object> dictionary)
        {
            var value = _getter(target);

            var dict = dictionary;
            for (int i = 0; i < Keys.Length - 1; i++)
            {
                var key = Keys[i];
                if (!dict.ContainsKey(key))
                {
                    var child = new Dictionary<string, object>();
                    dict.Add(key, child);
                    dict = child;
                }
                else
                {
                    dict = dict[key].As<IDictionary<string, object>>();
                }
            }

            dict.Add(Keys.Last(), value);
        }
    }
}