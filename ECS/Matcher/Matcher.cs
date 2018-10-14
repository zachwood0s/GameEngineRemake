using ECS.Components;
using ECS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Matching
{
    /// <summary>
    /// A Matcher is used to test an <see cref="Entity"/> for its
    /// component composition. There are various conditions like AllOf,
    /// NoneOf, AnyOf that it can test against and it can have a filter
    /// to test component properties as well. These operations can be chained
    /// together to get a complicated matching process
    /// 
    /// <example>
    /// An example of chaining match operations
    /// <code>
    /// new Matcher().AllOf(typeof(Component1), typeof(Component2))
    ///              .NoneOf(typeof(NoneOfComponent))
    ///              .AnyOf(typeof(Component3), typeof(Component4))
    /// </code>
    /// Cases where this matcher would succeed: (listing the components that are contained in an entity)
    /// - Component1, Component2, Component3
    /// - Component1, Component2, Component4
    /// - Component1, Component2, Component3, Component4
    ///
    /// Cases where this matcher would fail:
    /// - Component1
    /// - Component1, Component2, NoneOfComponent, Component3
    /// - Component1, Component2, NoneOfComponent, Component3
    /// </example>
    /// </summary>
    public class Matcher
    {
        private List<int> _allOfTypeIndicies;
        private List<Type> _allOf;

        private List<int> _anyOfTypeIndicies;
        private List<Type> _anyOf;

        private List<int> _noneOfTypeIndicies;
        private List<Type> _noneOf;

        private List<Predicate<Entity>> _filters;

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
            _filters = new List<Predicate<Entity>>();
        }

        #region Getters/Setters

        public IReadOnlyList<int> AllOfTypeIndicies => _allOfTypeIndicies;
        public IReadOnlyList<Type> AllOfTypes => _allOf;

        public IReadOnlyList<int> AnyOfTypeIndicies => _anyOfTypeIndicies;
        public IReadOnlyList<Type> AnyOfTypes => _anyOf;

        public IReadOnlyList<int> NoneOfTypeIndicies => _noneOfTypeIndicies;
        public IReadOnlyList<Type> NoneOfTypes => _noneOf;

        public IReadOnlyList<Predicate<Entity>> Filters => _filters;

        #endregion

        #region Adders
        /// <summary>
        /// Sets up the matcher to where an entity needs all of the
        /// provided component types to be a match. If the entity
        /// doesn't have one or more of the types, it will fail the
        /// match. The entity can have types other than the ones provided
        /// and still be a match.
        /// </summary>
        /// <param name="types">The component types that the entity needs to match all of</param>
        /// <returns>Returns itself so that method chaining is possibe</returns>
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

        /// <summary>
        /// Sets up the matcher to where an entity needs any of the provided
        /// component types to be a match. If the entity doesn't have any of the
        /// components, it will fail the match. If it has one or more, it will succeed.
        /// </summary>
        /// <param name="types">The component types that the entity needs to match any of</param>
        /// <returns>Returns itself so that method chaining is possibe</returns>
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

        /// <summary>
        /// Sets up the matcher to where an entity cannot have any of the 
        /// provided component types to be a match. If the entity has any of the components,
        /// it will fail the match. If it has none, it will succeed.
        /// </summary>
        /// <param name="types">The component types that the entity can have none of</param>
        /// <returns>Returns itself so that method chaining is possibe</returns>
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

        /// <summary>
        /// This is just a simpler way of writing 
        /// <code>new Matcher().AllOf(typeof(<typeparamref name="T"/>));</code>
        /// vs
        /// <code>new Matcher().Of<typeparamref name="T"/>();</code>
        /// </summary>
        /// <typeparam name="T">The type to match</typeparam>
        /// <returns>Returns itself so that method chaining is possibe</returns>
        public Matcher Of<T>()
        {
            return AllOf(typeof(T));
        }

        /// <summary>
        /// Filters allow you to match a condition along side the 
        /// typical matching of component types. You could test to
        /// make sure that a TransformComponent2D has an X position > 100
        /// or anything along those lines. Keep in mind, there will
        /// most likely be a slight performance hit by using these compared
        /// to regular matching, but if you're going to check the condition
        /// in the system anyway then I don't see a problem.
        /// </summary>
        /// <param name="conditions">The conditions that need to succeed to make a match</param>
        /// <returns>Returns itself so that method chaining is possibe</returns>
        public Matcher WithFilter(params Predicate<Entity>[] conditions)
        {
            if(conditions != null)
            {
                _filters.AddRange(conditions);
            }
            return this;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Matcher match && GetHashCode() == match.GetHashCode())
            {
                if (match._allOfTypeIndicies.Count != _allOfTypeIndicies.Count ||
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
                hash *= _filters.GetHashCode();
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
