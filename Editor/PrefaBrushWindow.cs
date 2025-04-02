#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.logandlp.prefabrush.editor
{
    using runtime;
    
    public class PrefaBrushWindow : EditorWindow
    {
        private const float MIN_SPAWN_RADIUS = .05f;
        private const float GUI_CIRCLE_DISTANCE = .5f;
        
        private static float _spawnRadius;
        
        private static Color _brushColor = Color.yellow;
        private static LayerMask _drawMask = ~0;
        private static InstanciateMode _instanciateMode = InstanciateMode.Range;
        
        private static PrefaBrushWindow _window;
        
        private static List<GameObject> _currentsPrefabsList = new();

        public static void OnSceneGUI(SceneView sceneView)
        {
            if (!_window.hasFocus/* && _currentsPrefabsList == null || _currentsPrefabsList.Count == 0 */)
                return;

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hitInfo) && _spawnRadius > MIN_SPAWN_RADIUS)
            {
                if (_drawMask != (_drawMask | (1 << hitInfo.transform.gameObject.layer)))
                    return;
                
                Handles.color = _brushColor;
                Handles.DrawWireDisc(hitInfo.point + hitInfo.normal * GUI_CIRCLE_DISTANCE, hitInfo.normal, _spawnRadius);
                Handles.DrawLine(hitInfo.point + hitInfo.normal * GUI_CIRCLE_DISTANCE, hitInfo.point);
            }
            
            sceneView.Repaint();
        }
        
        [MenuItem("Tools/PrefaBrush")]
        private static void ShowWindow()
        {
            GetWindow<PrefaBrushWindow>("PrefaBrush");
        }

        private void OnEnable()
        {
            _window = this;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            BrushOptionGUI();
        }

        private void BrushOptionGUI()
        {
            GUILayout.Label("Brush Options", EditorStyles.boldLabel);
            
            _brushColor = EditorGUILayout.ColorField("Brush Color", _brushColor);
            _drawMask = EditorGUILayout.MaskField("Drawing layer", _drawMask.value, UnityEditorInternal.InternalEditorUtility.layers);
            _spawnRadius = EditorGUILayout.FloatField("Spawn Radius", Mathf.Max(MIN_SPAWN_RADIUS, _spawnRadius));
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Spawning Options", EditorStyles.boldLabel);
            _instanciateMode = (InstanciateMode)EditorGUILayout.EnumPopup("Instanciate Mode", _instanciateMode);
        }
    }
}

#endif // UNITY_EDITOR