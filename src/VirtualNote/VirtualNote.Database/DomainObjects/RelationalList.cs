using System;
using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    sealed class RelationalList<TEntity> : ICollection<TEntity> where TEntity : class 
    {
        readonly IList<TEntity> _container = new List<TEntity>();

        readonly Action<TEntity> _insertedCallback;
        readonly Action<TEntity> _removedCallback;


        public RelationalList(Action<TEntity> insertedCallback)
        {
            if (insertedCallback == null)
                throw new InvalidOperationException();

            _insertedCallback = insertedCallback;
        }

        public RelationalList(Action<TEntity> insertedCallback, Action<TEntity> removedCallback) : this(insertedCallback)
        {
            _removedCallback = removedCallback;
        }

        /// <summary>
        ///     Try adds the item to the list.
        ///     If sucessfully added fires a insertCallback
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="InvalidOperationException">When the list already contains the item</exception>
        public void Add(TEntity item)
        {
            if(_container.Contains(item)){
                throw new InvalidOperationException();
            }
            _container.Add(item);
            
            // Fireup event
            _insertedCallback(item);
        }

        /// <summary>
        ///     Removes all elements on the list
        ///     This method will call removeCallback for each item on the list
        ///     This gives you the oportunity to remove the item from another relashionships
        /// </summary>
        public void Clear()
        {
            while (_container.Count > 0){
                TEntity e = _container[0]; // o(1)
                Remove(e);
            }
        }

        public bool Contains(TEntity item) {
            return _container.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex) {
            _container.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return _container.Count; }
        }

        public bool IsReadOnly {
            get { return _container.IsReadOnly; }
        }

        /// <summary>
        ///     If sucessfully removed fireup removedCallback
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if sucessfully removed, otherwise false</returns>
        public bool Remove(TEntity item) 
        {
            bool removed = _container.Remove(item);

            if (removed && _removedCallback != null){
                // fireup event
                _removedCallback(item);
            }
            return removed;
        }

        public IEnumerator<TEntity> GetEnumerator() {
            return _container.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _container.GetEnumerator();
        }
    }
}
