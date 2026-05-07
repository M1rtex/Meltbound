using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateWindTexture
{
    public static void Execute()
    {
        int width = 256;
        int height = 32;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedX = (float)x / width;
                float normalizedY = (float)y / height;

                // Создаём горизонтальную линию с градиентом по краям
                float fadeX = Mathf.SmoothStep(0f, 1f, normalizedX) * Mathf.SmoothStep(1f, 0f, normalizedX);
                fadeX = Mathf.Pow(fadeX, 0.5f) * 4f;
                fadeX = Mathf.Clamp01(fadeX);

                // Центральная линия
                float centerDist = Mathf.Abs(normalizedY - 0.5f) * 2f;
                float fadeY = 1f - Mathf.Pow(centerDist, 0.3f);
                fadeY = Mathf.Clamp01(fadeY);

                float alpha = fadeX * fadeY;

                Color color = new Color(1f, 1f, 1f, alpha);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        // Сохраняем текстуру
        string path = "Assets/Particle Systems/WindStreakTexture.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();

        // Настраиваем импорт текстуры
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        // Назначаем текстуру материалу
        Material windMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Particle Systems/WindMaterial.mat");
        if (windMaterial != null)
        {
            Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            windMaterial.SetTexture("_MainTex", loadedTexture);
            EditorUtility.SetDirty(windMaterial);
        }

        Debug.Log("Wind streak texture created at: " + path);
    }
}
