#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.logandlp.prefabrush.editor
{
    using runtime;
    
    public class PrefaBrushWindow : EditorWindow
    {
        private const int ITEMS_PER_LINE = 3;
        private const int MIN_NUMBER_INSTANCE = 1;
        private const int MIN_INCLINAITION_DEGREE = 0; 
        
        private const float MIN_SPAWN_RADIUS = .05f;
        private const float GUI_CIRCLE_DISTANCE = .5f;
        private const float MIN_SCALE_VARIATION_DEGREE = .05f;
        
        private static readonly Vector2 PREVIEW_SIZE = new(300, 100);
        private static readonly Vector2 ADD_PREVIEW_SIZE = new(90, 20);
        private static readonly Vector2 DELETE_PREVIEW_SIZE = new(20, 20);
        private static readonly Vector2 PREFABS_PREVIEW_SIZE = new(70, 70);

        private static int _fixedNumberInstance;
        private static int _RandomMinNumberInstance;
        private static int _RandomMaxNumberInstance;
        
        private static float _brushRadius;
        private static float _maxInclinationDregree;
        
        private static Color _brushColor = Color.yellow;
        private static Vector2 _scaleVariationSpawn;
        private static LayerMask _drawMask = ~0;
        private static InstanciateMode _instanciateMode = InstanciateMode.Range;
        
        private static List<GameObject> _currentsPrefabsList = new();
        private static List<GameObject> _selectedPrefabsList = new();
        
        private static PrefaBrushWindow _window;
        
        private Vector2 _scrollPositionList = Vector2.zero;

        public static void OnSceneGUI(SceneView sceneView)
        {
            if (!_window.hasFocus && _currentsPrefabsList == null || _currentsPrefabsList.Count == 0)
                return;

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hitInfo) && _brushRadius > MIN_SPAWN_RADIUS)
            {
                if (_drawMask != (_drawMask | (1 << hitInfo.transform.gameObject.layer)))
                    return;
                
                Handles.color = _brushColor;
                Handles.DrawWireDisc(hitInfo.point + hitInfo.normal * GUI_CIRCLE_DISTANCE, hitInfo.normal, _brushRadius);
                Handles.DrawLine(hitInfo.point + hitInfo.normal * GUI_CIRCLE_DISTANCE, hitInfo.point);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    SpawnObjects();
                    Event.current.Use();
                }
            }
            
            sceneView.Repaint();
        }
        
        private static void SpawnObjects()
        {
            Debug.Log("okay");
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
            // save
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            // save
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
            _brushRadius = EditorGUILayout.FloatField("Brush Radius", Mathf.Max(MIN_SPAWN_RADIUS, _brushRadius));
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Spawning Options", EditorStyles.boldLabel);
            _instanciateMode = (InstanciateMode)EditorGUILayout.EnumPopup("Instanciate Mode", _instanciateMode);

            switch (_instanciateMode)
            {
                case InstanciateMode.Fixed:
                    _fixedNumberInstance = EditorGUILayout.IntField("Number Instance", Mathf.Max(MIN_NUMBER_INSTANCE, _fixedNumberInstance));
                    break;
                case InstanciateMode.Range:
                    _RandomMinNumberInstance = EditorGUILayout.IntField("Min Number Instance", Mathf.Max(MIN_NUMBER_INSTANCE, _RandomMinNumberInstance));
                    _RandomMaxNumberInstance = EditorGUILayout.IntField("Max Number Instance", Mathf.Max(_RandomMinNumberInstance, _RandomMaxNumberInstance));
                    break;
            }
            _maxInclinationDregree = EditorGUILayout.FloatField("Max Inclination Dregree", Mathf.Max(MIN_INCLINAITION_DEGREE, _maxInclinationDregree));
            _scaleVariationSpawn = EditorGUILayout.Vector2Field("Scale Variation Spawn", new (Mathf.Max(MIN_SCALE_VARIATION_DEGREE, _scaleVariationSpawn.x), Mathf.Max(MIN_SCALE_VARIATION_DEGREE, _scaleVariationSpawn.y)));
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefabs", EditorStyles.boldLabel);

            if (GUILayout.Button("+ Add Prefab", GUILayout.Width(ADD_PREVIEW_SIZE.x), GUILayout.Height(ADD_PREVIEW_SIZE.y)))
            {
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, string.Empty, 0);
            }
            EditorGUILayout.EndHorizontal();
            
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                GameObject selectedPrefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                if (selectedPrefab != null && !_selectedPrefabsList.Contains(selectedPrefab))
                {
                    _selectedPrefabsList.Add(selectedPrefab);
                }
            }

            int lineCount = 0;
            List<GameObject> removeItemList = new();
            
            _scrollPositionList = EditorGUILayout.BeginScrollView(_scrollPositionList, GUI.skin.box, GUILayout.Height(PREVIEW_SIZE.x));

            foreach (GameObject selectedPrefabs in _selectedPrefabsList)
            {
                if (lineCount == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                
                Texture2D selectedPrefabsTexture = AssetPreview.GetAssetPreview(selectedPrefabs);
                using (new GUILayout.VerticalScope(GUILayout.Width(PREVIEW_SIZE.y)))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        Color originalBackgroundColor = GUI.color;
                        if (_currentsPrefabsList.Contains(selectedPrefabs))
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        
                        if (GUILayout.Button(selectedPrefabsTexture, GUILayout.Width(PREFABS_PREVIEW_SIZE.x), GUILayout.Height(PREFABS_PREVIEW_SIZE.y)))
                        {
                            if (_currentsPrefabsList.Contains(selectedPrefabs))
                            {
                                _currentsPrefabsList.Remove(selectedPrefabs);
                            }
                            else
                            {
                                _currentsPrefabsList.Add(selectedPrefabs);
                            }
                        }
                        
                        GUI.backgroundColor = originalBackgroundColor;

                        if (GUILayout.Button("X", GUILayout.Width(DELETE_PREVIEW_SIZE.x), GUILayout.Height(DELETE_PREVIEW_SIZE.y)))
                        {
                            removeItemList.Add(selectedPrefabs);
                        }
                    }
                }
                lineCount++;
                if (lineCount >= ITEMS_PER_LINE)
                {
                    EditorGUILayout.EndHorizontal();
                    lineCount = 0;
                }
            }
            EditorGUILayout.EndScrollView();
            if (lineCount > 0)
            {
                EditorGUILayout.EndHorizontal();
            }

            foreach (GameObject removeItem in removeItemList)
            {
                _selectedPrefabsList.Remove(removeItem);
                if (_currentsPrefabsList.Contains(removeItem))
                {
                    _currentsPrefabsList.Remove(removeItem);
                }
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    foreach (Object objectReference in DragAndDrop.objectReferences)
                    {
                        GameObject objectReferencePrefabs = objectReference as GameObject;
                        if (objectReferencePrefabs != null && !_selectedPrefabsList.Contains(objectReferencePrefabs))
                        {
                            _selectedPrefabsList.Add(objectReferencePrefabs);
                        }
                    }
                    Event.current.Use();
                    break;
            }
            
            Repaint();
        }
    }
}

#endif // UNITY_EDITOR