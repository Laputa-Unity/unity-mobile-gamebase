#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagFilterView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabTagFilter _prefabTagFilter;
        #endregion

        #region Constructors
        public PrefabTagFilterView(PrefabTagFilter prefabTagFilter)
        {
            _prefabTagFilter = prefabTagFilter;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _prefabTagFilter.NameFilter.View.Render();
        }
        #endregion
    }
}
#endif