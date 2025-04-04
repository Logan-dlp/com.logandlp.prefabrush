using System.Collections.Generic;
using UnityEngine;

namespace com.logandlp.prefabrush.runtime
{
    [System.Serializable]
    public struct PrefaBrushData
    {
        public int FixedNumberInstance;
        public int RandomMinNumberInstance;
        public int RandomMaxNumberInstance;
        
        public float BrushRadius;
        public float MaxInclinationDregree;
        
        public LayerMask DrawMask;
        public InstanciateMode InstanciateMode;
        
        public System.Numerics.Vector4 BrushColor;
        public System.Numerics.Vector2 ScaleVariationSpawn;
        
        public List<string> SelectedPrefabsPathList;
        public List<string> CurrentPrefabsPathList;
    }
}