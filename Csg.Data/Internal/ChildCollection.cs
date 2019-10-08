using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Internal
{
    public class ChildCollection<T> : System.Collections.Generic.ICollection<T>
    {
        private readonly ICollection<T> _parent;
        protected List<T> _child;

        public ChildCollection(ICollection<T> parent)
        {
            _parent = parent;
        }

        public int Count
        {
            get
            {
                return _parent.Count + (_child == null ? 0 : _child.Count);
            }
        }

        protected List<T> EnsureChild()
        {
            _child = _child ?? new List<T>();
            return _child;
        }

        #region ICollection

        public void Add(T item)
        {
            _child = _child ?? new List<T>();
            _child.Add(item);
        }

        public bool Contains(T item)
        {
            return _parent.Contains(item) || (_child?.Contains(item) == true);
        }

        void ICollection<T>.Clear()
        {
            _child?.Clear();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _parent.CopyTo(array, 0);
            _child?.CopyTo(array, arrayIndex + _parent.Count);
        }

        bool ICollection<T>.Remove(T item)
        {
            return (_child?.Remove(item) == true);
        }

        bool ICollection<T>.IsReadOnly => false;

        #endregion

        #region "IEnumerable"

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new ChildEnumerator<T>(_parent.GetEnumerator(), _child?.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ChildEnumerator<T>(_parent.GetEnumerator(), _child?.GetEnumerator());
        }

        #endregion

        public class ChildEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _parent;
            private readonly IEnumerator<T> _child;
            private bool _parentComplete = false;
            private T _current;

            public ChildEnumerator(IEnumerator<T> parent, IEnumerator<T> child)
            {
                _parent = parent;
                _child = child;
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose()
            {
                _parent.Dispose();
                if (_child != null)
                {
                    _child.Dispose();
                }
            }

            public bool MoveNext()
            {
                if (_parentComplete && _child == null)
                {
                    _current = default(T);
                }
                else if (_parentComplete)
                {
                    _child.MoveNext();
                    _current = _child.Current;
                }
                else
                {
                    _parentComplete = (_parent.MoveNext() == false);
                    _current = _parent.Current;
                }

                return _current != null;
            }

            public void Reset()
            {
                _parent.Reset();
                if (_child != null)
                {
                    _child.Reset();
                }
            }
        }
    }

    public class ChildList<T> : ChildCollection<T>, IList<T>
    {
        private readonly IList<T> _parentList;

        public ChildList(IList<T> parent) : base(parent)
        {
            _parentList = parent;
        }

        public T this[int index] 
        {
            get => index < _parentList.Count ? _parentList[index] : _child[index - _parentList.Count];
            set
            {
                if (index < _parentList.Count)
                {
                    _parentList[index] = value;
                }
                else
                {
                    _child[index - _parentList.Count] = value;
                }
            }
        }

        public int IndexOf(T item)
        {
            var idx = _parentList.IndexOf(item);
            if (idx >= 0)
            {
                return idx;
            }
            else if (_child != null && (idx = _child.IndexOf(item)) >= 0)
            {
                return _parentList.Count + idx;
            }
            else
            {
                return -1;
            }
        }

        public void Insert(int index, T item)
        {
            if (index <= _parentList.Count)
            {
                _parentList.Insert(index, item);
            }
            else
            {
                EnsureChild().Insert(index - _parentList.Count, item);
            }
        }

        public void RemoveAt(int index)
        {
            if (index <= _parentList.Count)
            {
                _parentList.RemoveAt(index);
            }
            else if(_child != null)
            {
                _child.RemoveAt(_parentList.Count - index);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
