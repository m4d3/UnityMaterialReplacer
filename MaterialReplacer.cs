using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialReplacer : EditorWindow
{
    private Material _baseMaterial;
    private Material _replaceMaterial;
    private GameObject[] _selection;

    private GUIStyle _style;
    private string _text;

    [MenuItem("Window/MaterialReplacer")]
    private static void Create() {
        MaterialReplacer window = (MaterialReplacer)GetWindow(typeof(MaterialReplacer));
        window.Show();
    }

    private void Awake() {
        _style = new GUIStyle();
        _style.wordWrap = true;

        _text = "Select one or more objects, if base material is found it will be replaced by replace material";
    }

    private void OnGUI() {
        EditorGUILayout.BeginVertical();

        _baseMaterial = (Material) EditorGUILayout.ObjectField("Base Material", _baseMaterial, typeof(Material), false);
        _replaceMaterial = (Material)EditorGUILayout.ObjectField("Replace Material", _replaceMaterial, typeof(Material), false);

        if (GUILayout.Button("Replace Materials"))
        {
            if (!_baseMaterial)
            {
                _text = "No base material selected, please fill base material slot";
                return;
            }
            if (!_replaceMaterial)
            {
                _text = "No replace material selected, please fill replace material slot";
                return;
            }
            _selection = Selection.gameObjects;
            if (_selection.Length > 0)
            {
                foreach (GameObject go in _selection)
                {
                    SwitchMaterials(go);
                    SwitchChildMaterials(go.transform);
                }
            }
            else
            {
                _text =
                    "No object selected, please select one or more objects that should have their materials replaced.";
            }
        }
        EditorGUILayout.LabelField(_text, _style);

        EditorGUILayout.EndVertical();
    }

    void SwitchMaterials(GameObject go)
    {
        if (go.GetComponent<MeshRenderer>()) {
            Material[] sharedMats = go.GetComponent<MeshRenderer>().sharedMaterials;
            for (int i = 0; i < sharedMats.Length; i++) {
                if (sharedMats[i].name.Equals(_baseMaterial.name)) {
                    sharedMats[i] = _replaceMaterial;
                }
            }

            go.GetComponent<MeshRenderer>().sharedMaterials = sharedMats;
        }
    }

    void SwitchChildMaterials(Transform root)
    {
        foreach (Transform child in root) {
            SwitchMaterials(child.gameObject);
            SwitchChildMaterials(child);
        }
    }
}
