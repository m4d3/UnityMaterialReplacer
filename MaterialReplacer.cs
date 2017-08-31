using UnityEditor;
using UnityEngine;

public class MaterialReplacer : EditorWindow
{
    private Material _baseMaterial;
    private int _counter;
    private Material _replaceMaterial;
    private GameObject[] _selection;

    private GUIStyle _style;
    private string _text;

    [MenuItem("Window/MaterialReplacer")]
    private static void Create()
    {
        var window = (MaterialReplacer) GetWindow(typeof(MaterialReplacer));
        window.Show();
    }

    private void Awake()
    {
        _style = new GUIStyle();
        _style.wordWrap = true;

        _text = "Select one or more objects, if base material is found it will be replaced by replace material";
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        _baseMaterial = (Material) EditorGUILayout.ObjectField("Base Material", _baseMaterial, typeof(Material), false);
        _replaceMaterial =
            (Material) EditorGUILayout.ObjectField("Replace Material", _replaceMaterial, typeof(Material), false);

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
                _counter = 0;
                foreach (var go in _selection)
                {
                    SwitchMaterials(go);
                    SwitchChildMaterials(go.transform);
                }

                _text = _counter + " materials replaced";
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

    private void SwitchMaterials(GameObject go)
    {
        if (go.GetComponent<MeshRenderer>())
        {
            var sharedMats = go.GetComponent<MeshRenderer>().sharedMaterials;
            if (sharedMats != null && sharedMats.Length > 0)
            {
                for (var i = 0; i < sharedMats.Length; i++)
                    if (sharedMats[i] != null && sharedMats[i].name.Equals(_baseMaterial.name))
                    {
                        sharedMats[i] = _replaceMaterial;
                        _counter++;
                    }

                go.GetComponent<MeshRenderer>().sharedMaterials = sharedMats;
            }
        }
    }

    private void SwitchChildMaterials(Transform root)
    {
        foreach (Transform child in root)
        {
            SwitchMaterials(child.gameObject);
            SwitchChildMaterials(child);
        }
    }
}