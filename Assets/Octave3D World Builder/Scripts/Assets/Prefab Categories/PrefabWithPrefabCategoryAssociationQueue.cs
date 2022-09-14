#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabWithPrefabCategoryAssociationQueue : Singleton<PrefabWithPrefabCategoryAssociationQueue>
    {
        #region Private Variables
        private Queue<PrefabWithPrefabCategoryAssociation> _associationQueue = new Queue<PrefabWithPrefabCategoryAssociation>();
        #endregion

        #region Public Methods
        public void Enqueue(PrefabWithPrefabCategoryAssociation association)
        {
            if(association != null) _associationQueue.Enqueue(association);
        }

        public void DequeueAndPerform()
        {
            if (_associationQueue.Count != 0)
            {
                PrefabWithPrefabCategoryAssociation association = _associationQueue.Dequeue();
                association.Perform();
            }
        }

        public void DequeAllAndPerform()
        {
            while (_associationQueue.Count != 0)
            {
                DequeueAndPerform();
            }

            Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
        }
        #endregion
    }
}
#endif