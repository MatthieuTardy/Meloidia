using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoTextureMaterialProcessor : AssetPostprocessor
{
    private const string TARGET_FOLDER = "Assets/Import_Asset/Texture";
    private const string TEMPLATE_MATERIAL_PATH = "Assets/Import_Asset/Material/m_pont.mat";
    private const string MATERIAL_OUTPUT_FOLDER = "Assets/Import_Asset/Material";

    private const string TEXTURE_PROPERTY_NAME = "MainTex";

    void OnPreprocessTexture()
    {
        if (assetPath.StartsWith(TARGET_FOLDER))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            string cleanPath = path.Replace("\\", "/");

            if (cleanPath.StartsWith(TARGET_FOLDER) &&
               (cleanPath.EndsWith(".png") || cleanPath.EndsWith(".jpg") || cleanPath.EndsWith(".jpeg") || cleanPath.EndsWith(".tga")))
            {
                EditorApplication.delayCall += () => GenererMaterial(cleanPath);
            }
        }
    }

    private static void GenererMaterial(string texturePath)
    {
        Material templateMat = AssetDatabase.LoadAssetAtPath<Material>(TEMPLATE_MATERIAL_PATH);

        if (templateMat == null)
        {
            Debug.LogError($"Template material not found at path: {TEMPLATE_MATERIAL_PATH}");
            return;
        }

        if (!AssetDatabase.IsValidFolder(MATERIAL_OUTPUT_FOLDER))
        {
            string parentFolder = Path.GetDirectoryName(MATERIAL_OUTPUT_FOLDER).Replace("\\", "/");
            string newFolder = Path.GetFileName(MATERIAL_OUTPUT_FOLDER);
            AssetDatabase.CreateFolder(parentFolder, newFolder);
        }

        string textureName = Path.GetFileNameWithoutExtension(texturePath);
        string materialPath = $"{MATERIAL_OUTPUT_FOLDER}/{textureName}_Mat.mat";

        if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) != null) return;

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (tex == null) return;

        Material newMat = new Material(templateMat);

        if (newMat.HasProperty(TEXTURE_PROPERTY_NAME))
        {
            newMat.SetTexture(TEXTURE_PROPERTY_NAME, tex);
            Debug.Log($"Texture {textureName} applied successfully on {TEXTURE_PROPERTY_NAME}");
        }
        else
        {
            Debug.LogWarning($"WARNING: The material shader does not have a property named '{TEXTURE_PROPERTY_NAME}'. The texture could not be assigned.");
            newMat.mainTexture = tex;
        }

        AssetDatabase.CreateAsset(newMat, materialPath);
        AssetDatabase.SaveAssets();
    }
}