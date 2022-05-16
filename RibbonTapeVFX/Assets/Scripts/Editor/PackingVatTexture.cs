using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class PackingVatTexture : EditorWindow
{
    public List<Texture2D> sources = new List<Texture2D>();
    
    [MenuItem("Window/RibbonTapeVFX/VatTexturePacker")]
    public static void ShowExample()
    {
        var wnd = GetWindow<PackingVatTexture>();
        wnd.titleContent = new GUIContent("VatTexturePacker");
    }

    public void CreateGUI()
    {
        sources = new List<Texture2D>();
        
        var root = rootVisualElement;
        var visualTreeAssets = Resources.Load<VisualTreeAsset>("VatTexturePacker");
        visualTreeAssets.CloneTree(root);

        var sourcesElem = root.Q<ListView>("sources");
        sourcesElem.itemsSource = sources;
        sourcesElem.makeItem = () =>
        {
            Debug.Log("Make element");
            var textureElem = new ObjectField("Source Texture2D");
            textureElem.objectType = typeof(Texture2D);
            textureElem.allowSceneObjects = false;
            textureElem.RegisterValueChangedCallback(e =>
            {
                Debug.Log("Select element");
                var index = (int) textureElem.userData;
                if (sources.Count <= index)
                {
                    Debug.Log("Out of Bounds");
                    return;
                }
                
                sources[index] = e.newValue as Texture2D;
                textureElem.label = sources[index] != null ? sources[index].name : "Null";

                foreach (var source in sources)
                {
                    Debug.Log(source);
                }
            });
            return textureElem;
        };
        sourcesElem.bindItem = (elem, index) =>
        {
            if (elem is ObjectField textureElem)
            {
                textureElem.value = sources[index];
                textureElem.label = sources[index] != null ? sources[index].name : "Null";
            }

            if (sourcesElem.Children() is ObjectField[] children)
            {
                for (var i = 0; i < children.Length; i++)
                {
                    children[i].value = sources[i];
                    children[i].label = sources[i] != null ? sources[i].name : "Null";
                }
            }
            elem.userData = index;
        };

        var packingButton = root.Q<Button>("packingButton");
        packingButton.clicked += () =>
        {
            var str = new StringBuilder();
            for (var i = 0; i < sources.Count; i++)
            {
                str.Append(sources[i]?.name ?? "Null");
                str.Append("\n");
            }
            Debug.Log(str);
        };
    }
}