using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public enum GrabType
{
    SPINDLE_GRAB = 0,
    CUSTOM = 1,
}

public class ArtifactImportWindow : EditorWindow
{
    static ArtifactImportWindow window;

    string artifactName = "New Artifact";
    string artifactModelPath = "";

    string modelSavePath = "Assets/Models";
    GrabType grabLogicType;
    ExhibitManagerSO exhibitManager;
    GameObject canvasPrefab;

    bool showAdvancedSettings = false;

    string slideShowPath = "";
    bool generateWithSlideShow = true;

    [MenuItem("Exhibit Creator/Artifact Importer")]
    public static void ShowWindow()
    {
        window = (ArtifactImportWindow)GetWindow(typeof(ArtifactImportWindow));
        window.titleContent = new GUIContent("Artifact Importer");
        window.Init();
        window.Show();
    }

    void Init()
    {

    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        string newName = EditorGUILayout.TextField("Artifact Name: ", artifactName);
        if (newName != artifactName)
        {
            artifactName = newName;
        }

        GUILayout.Space(10);

        GUILayoutOption[] ops = { };
        exhibitManager = (ExhibitManagerSO)EditorGUILayout.ObjectField("Exhibit Manager:", exhibitManager, typeof(ExhibitManagerSO), false, ops);
        if (!exhibitManager)
        {
            DrawCreateExhibitManager();
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        string newPath = EditorGUILayout.TextField("Artifact Import Path: ", artifactModelPath);
        if (newPath != artifactModelPath)
        {
            artifactModelPath = newPath;
        }

        GUILayoutOption[] ops2 = { GUILayout.MaxWidth(50) };
        if (GUILayout.Button("Set", ops2))
        {
            SelectArtifactModel();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        bool newBool = EditorGUILayout.Toggle("Generate With Slideshow: ", generateWithSlideShow);
        if (newBool != generateWithSlideShow)
        {
            generateWithSlideShow = newBool;
        }

        if (generateWithSlideShow)
        {
            DrawChooseSlideShowFolder();
        }
        else
        {
            DrawLinkArtifactDisplay();
        }

        GUILayout.Space(10);

        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
        if (showAdvancedSettings)
        {
            DrawAdvancedSettings();
        }

        GUILayout.Space(10);

        if (!exhibitManager || artifactModelPath.Length < 1 || artifactName.Length < 1 || 
            (generateWithSlideShow && slideShowPath.Length < 1))
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Import Artifact"))
        {
            CreatePrefab();
        }

        GUI.enabled = true;

    }

    void DrawChooseSlideShowFolder()
    {
        GUILayout.BeginHorizontal();
        string newPath = EditorGUILayout.TextField("Slideshow Import Folder: ", slideShowPath);
        GUILayoutOption[] ops = { GUILayout.MaxWidth(50) };
        if (newPath != slideShowPath)
        {
            slideShowPath = newPath;
        }

        if (GUILayout.Button("Set", ops))
        {
            SelectPresentationFolder();
        }
        GUILayout.EndHorizontal();
    }

    void DrawAdvancedSettings()
    {
        EditorGUI.indentLevel++;

        GUILayoutOption[] ops = { GUILayout.MaxWidth(50) };

        GUILayout.BeginHorizontal();
        GUILayoutOption[] ops2 = { };
        string[] types = { "Default", "Custom" };
        grabLogicType = (GrabType)EditorGUILayout.Popup("Artifact Interaction Behavior", (int)grabLogicType, types, ops2);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        modelSavePath = EditorGUILayout.TextField("Model Save Path", modelSavePath);
        if (GUILayout.Button("Set", ops))
        {
            string newModelPath = EditorUtility.OpenFolderPanel("Choose Model Save Folder", "Assets", "");
            if (newModelPath.Length > 0)
            {
                if (!newModelPath.StartsWith(Application.dataPath))
                {
                    Debug.LogError("Modle save path must be within Assets folder");
                    return;
                }

                newModelPath = "Assets" + newModelPath.Substring(Application.dataPath.Length);

                modelSavePath = newModelPath;
            }
        }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    void DrawCreateExhibitManager()
    {
        if (GUILayout.Button("Create Exhibit Manager"))
        {
            string path = EditorUtility.OpenFolderPanel("Choose where to save the exhibit manager", "Assets", "");
            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogError("Cannot create manager outside of Assets folder");
                return;
            }

            exhibitManager = CreateInstance<ExhibitManagerSO>();
            AssetDatabase.CreateAsset(exhibitManager, path + "/exhibitManagerSO.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void DrawLinkArtifactDisplay()
    {
        GUILayoutOption[] ops = { GUILayout.MaxWidth(50) };
        GUILayoutOption[] ops2 = { };

        GUILayout.BeginHorizontal();
        canvasPrefab = (GameObject)EditorGUILayout.ObjectField("Link Artifact Display: ", canvasPrefab, typeof(GameObject), false, ops2);
        if (!canvasPrefab && GUILayout.Button("Create", ops))
        {
            string savePath = EditorUtility.OpenFolderPanel("Choose folder to save it in", "Assets", "");
            if (savePath.Length > 0)
            {
                if (!savePath.StartsWith(Application.dataPath))
                {
                    Debug.LogError("Save path must be within Assets folder");
                    return;
                }

                savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
                GameObject canv = new GameObject();
                canv.AddComponent<Canvas>();
                bool success;
                canvasPrefab = PrefabUtility.SaveAsPrefabAsset(canv, savePath + "/defaultCanvas.prefab", out success);
                DestroyImmediate(canv);
            }
        }
        GUILayout.EndHorizontal();
    }

    void SelectArtifactModel()
    {
        string newPath = EditorUtility.OpenFilePanel("Select Artifact Model", "", "fbx,obj,dae,3ds,dxf");
        if (newPath.Length > 0)
        {
            artifactModelPath = newPath;
        }
    }

    void SelectPresentationFolder()
    {
        string newPath = EditorUtility.OpenFolderPanel("Select Slideshow Folder", "", "");
        if (newPath.Length > 0)
        {
            slideShowPath = newPath;
        }
    }

    void CreatePrefab()
    {
        // first verify that we have a model to import
        if (!File.Exists(artifactModelPath))
        {
            Debug.LogError("Cannot find model at " + artifactModelPath);
            return;
        }

        if (generateWithSlideShow && !Path.IsPathFullyQualified(slideShowPath))
        {
            Debug.LogError("Cannot load slide show at " + slideShowPath);
            return;
        }

        // then get save path
        bool gotSave;
        string savePath = GetSavePath(out gotSave);
        if (!gotSave) return;

        // now copy model into project and get relative path
        string modelPath = GetModelProjectPath(out gotSave);
        if (!gotSave) return;

        GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(modelPath, typeof(GameObject));
        
        if (!go)
        {
            Debug.LogError("Could not load artifact model");
            return;
        }

        GameObject sceneObj = Instantiate(go);
        SetObjectProperties(sceneObj, savePath);

        bool success;
        PrefabUtility.SaveAsPrefabAsset(sceneObj, savePath + "/" + artifactName + ".prefab", out success);
        DestroyImmediate(sceneObj);
        if (!success)
        {
            Debug.LogError( "Failed to save prefab to " + savePath + "/" + artifactName + ".prefab");
            return;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Import Successful", "Artifact Successfully Imported!", "OK");

        ClearFields();
    }

    void SetObjectProperties(GameObject obj, string savePath)
    {
        obj.name = artifactName;
        obj.AddComponent<SphereCollider>();

        Artifact art = obj.AddComponent<Artifact>();
        art.exhibitManager = exhibitManager;
        art.canvasPrefab = GenerateCanvasPrefab(savePath);

        if (grabLogicType == GrabType.SPINDLE_GRAB)
        {
            obj.AddComponent<SpindleGrabLogic>();
        }
    }

    GameObject GenerateCanvasPrefab(string savePath)
    {
        GameObject canvPrefab, temp;

        if (canvasPrefab)
        {
            temp = Instantiate(canvasPrefab);
        }
        else
        {
            temp = new GameObject();
            temp.AddComponent<Canvas>();
        }

        bool success;
        canvPrefab = PrefabUtility.SaveAsPrefabAsset(temp, savePath + "/" + artifactName + "_canvas.prefab", out success);
        DestroyImmediate(temp);

        if (!success)
        {
            Debug.LogError("Could not save canvas as prefab");
        }

        return canvPrefab;
    }

    string GetSavePath(out bool success)
    {
        string savePath = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
        if (savePath.Length == 0)
        {
            success = false;
            return "";
        }

        if (!savePath.StartsWith(Application.dataPath))
        {
            Debug.LogError("Must save artifact somewhere inside the project Assets folder");
            success = false;
            return "";
        }

        if (File.Exists(savePath + "/" + artifactName))
        {
            if (!EditorUtility.DisplayDialog("An artifact already exists with this name", "Overwrite", "Cancel"))
            {
                success = false;
                return "";
            }
        }

        success = true;
        return "Assets" + savePath.Substring(Application.dataPath.Length);
    }

    string GetModelProjectPath(out bool success)
    {
        string extension = artifactModelPath.Substring(artifactModelPath.LastIndexOf('.'));
        string modelPath = modelSavePath + "/" + artifactName + "_model" + extension;
        string fullModelPath = Application.dataPath + modelPath.Substring(6);
        int ind = 1;

        while (File.Exists(fullModelPath))
        {
            modelPath = modelSavePath + "/" + artifactName + "_model" + ind + extension;
            fullModelPath = Application.dataPath + modelPath.Substring(6);
            ind++;
        }

        // and now copy model into the project
        File.Copy(artifactModelPath, fullModelPath);
        AssetDatabase.ImportAsset(modelPath);

        success = true;
        return modelPath;
    }

    void GenerateFolders(string path)
    {
        // ex) path = "Assets/Prefabs/Artifacts/SOs"
        // folders = ["Assets", "Prefabs", "Artifacts", "SOs"]
        string[] folders = path.Split("/");

        List<string> parents = new List<string>();
        string cur = path;

        while (cur.Length > 0)
        {
            parents.Add(cur);
            int ind = cur.LastIndexOf('/');
            cur = cur.Substring(0, ind > 0 ? ind : 0);
        }
                
        // parents = ["Assets", "Assets/Prefabs", "Assets/Prefabs/Artifacts", "Assets/Prefabs/Artifacts/SOs"]
        parents.Reverse();

        for (int i = 0; i < parents.Count-1; i++)
        {
            // check if we need to create a subfolder with current parent
            if (!AssetDatabase.IsValidFolder(parents[i + 1]))
            {
                AssetDatabase.CreateFolder(parents[i], folders[i + 1]);
            }
        }
    }

    void ClearFields()
    {
        artifactName = "New Artifact";
        artifactModelPath = "";
    }
}
