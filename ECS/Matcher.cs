using ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Matcher
    {
        private List<int> _allOfTypeIndicies;
        private List<Type> _allOf;

        private List<int> _anyOfTypeIndicies;
        private List<Type> _anyOf;

        private List<int> _noneOfTypeIndicies;
        private List<Type> _noneOf;

        private bool _isHashCached;
        private int _cachedHash;

        public Matcher()
        {
            _allOfTypeIndicies = new List<int>();
            _anyOfTypeIndicies = new List<int>();
            _noneOfTypeIndicies = new List<int>();

            _anyOf = new List<Type>();
            _noneOf = new List<Type>();
            _allOf = new List<Type>();
        }

        #region Getters/Setters

        public List<int> AllOfTypeIndicies => _allOfTypeIndicies;
        public List<Type> AllOfTypes => _allOf;

        public List<int> AnyOfTypeIndicies => _anyOfTypeIndicies;
        public List<Type> AnyOfTypes => _anyOf;

        public List<int> NoneOfTypeIndicies => _noneOfTypeIndicies;
        public List<Type> NoneOfTypes => _noneOf;

        #endregion

        #region Adders
        public Matcher AllOf(params Type[] types)
        {
            if(types != null)
            {
                foreach(Type type in types)
                {
                    _allOf.Add(type);
                    int componentIndex = ComponentPool.GetComponentIndex(type);
                    if (componentIndex != -1)
                    {
                        _allOfTypeIndicies.Add(componentIndex);
                    }
                }
            }

            return this;
        }

        public Matcher AnyOf(params Type[] types)
        {
            if(types != null)
            {
                foreach(Type type in types)
                {
                    _anyOf.Add(type);
                    int componentIndex = ComponentPool.GetComponentIndex(type);
                    if (componentIndex != -1)
                    {
                        _anyOfTypeIndicies.Add(componentIndex);
                    }
                }
            }
            return this;
        }
        public Matcher NoneOf(params Type[] types)
        {
            if(types != null)
            {
                foreach(Type type in types)
                {
                    _noneOf.Add(type);
                    int componentIndex = ComponentPool.GetComponentIndex(type);
                    if (componentIndex != -1)
                    {
                        _noneOfTypeIndicies.Add(componentIndex);
                    }
                }
            }
            return this;
        }

        public Matcher Of<T>()
        {
            return AllOf(typeof(T));
        }

        #endregion

        public override bool Equals(object obj)
        {
            Matcher match = obj as Matcher;

            if(match != null && GetHashCode() == match.GetHashCode())
            {
                if(match._allOfTypeIndicies.Count != _allOfTypeIndicies.Count ||
                   match._anyOfTypeIndicies.Count != _anyOfTypeIndicies.Count ||
                   match._noneOfTypeIndicies.Count != _noneOfTypeIndicies.Count)
                {
                    return false;
                }
                return _allOfTypeIndicies.All(match._allOfTypeIndicies.Contains) && _noneOfTypeIndicies.All(match._noneOfTypeIndicies.Contains) && _anyOfTypeIndicies.All(match._anyOfTypeIndicies.Contains);
            }

            return false;
        }

        #region HashCode 
        public override int GetHashCode()
        {
            if(!_isHashCached)
            {
                var hash = GetType().GetHashCode();
                hash = _ApplyHash(hash, _allOfTypeIndicies.ToArray(), 3, 53);
                hash = _ApplyHash(hash, _anyOfTypeIndicies.ToArray(), 307, 367);
                hash = _ApplyHash(hash, _noneOfTypeIndicies.ToArray(), 647, 683);
                _cachedHash = hash;
                _isHashCached = true;
            }
            return _cachedHash;
        }
        private static int _ApplyHash(int hash, int[] indices, int i1, int i2)
        {
            if (indices != null)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    hash ^= indices[i] * i1;
                }
                hash ^= indices.Length * i2;
            }

            return hash;
        }

        #endregion
    }
}
