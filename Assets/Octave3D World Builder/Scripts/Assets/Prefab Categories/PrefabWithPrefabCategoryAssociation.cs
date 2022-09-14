#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class PrefabWithPrefabCategoryAssociation
    {
        #region Private Variables
        private PrefabCategory _prefabCategory;
        private Prefab _prefab;
        #endregion

        #region Public Properties
        public PrefabCategory PrefabCategory { get { return _prefabCategory; } set { if (value != null) _prefabCategory = value; } }
        public Prefab Prefab { get { return _prefab; } set { if (value != null) _prefab = value; } }
        #endregion

        #region Public Methods
        public void Perform()
        {
            UndoEx.RecordForToolAction(_prefabCategory);
            _prefabCategory.AddPrefab(_prefab);
        }
        #endregion
    }
}
#endif