using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StudentImporter : EditorWindow
{
    private TextAsset csvFile;
    private string studentSavePath = "Assets/Resources/StudentSO";
    private string featurePath = "Assets/Resources/FeatureSO";

    [MenuItem("Tools/Import Students From CSV")]
    public static void ShowWindow()
    {
        GetWindow<StudentImporter>("Student Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Student CSV Importer", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        studentSavePath = EditorGUILayout.TextField("Student Save Path", studentSavePath);
        featurePath = EditorGUILayout.TextField("Feature Path", featurePath);

        if (GUILayout.Button("Import"))
        {
            if (csvFile != null)
            {
                ImportStudents();
            }
            else
            {
                Debug.LogError("CSV 파일을 선택하세요!");
            }
        }
    }

    private void ImportStudents()
    {
        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 줄은 헤더
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            if (values.Length < 2) continue;

            string id = values[0].Trim();
            string label = values[1].Trim();

            // StudentSO 생성
            StudentSO student = ScriptableObject.CreateInstance<StudentSO>();
            student.id = id;
            student.label = label;
            student.sprite = $"Portrait/{id}";
            student.features = new List<FeatureSO>();

            // Feature1 ~ Feature6 탐색
            for (int j = 2; j < values.Length; j++)
            {
                string featureName = values[j].Trim();
                if (string.IsNullOrEmpty(featureName)) continue;

                // FeatureSO 에셋 찾기
                string[] guids = AssetDatabase.FindAssets(featureName + " t:FeatureSO", new[] { featurePath });
                if (guids.Length > 0)
                {
                    string featureAssetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    FeatureSO feature = AssetDatabase.LoadAssetAtPath<FeatureSO>(featureAssetPath);
                    if (feature != null)
                    {
                        if(!student.features.Contains(feature))
                            student.features.Add(feature);
                    }
                }
                else
                {
                    Debug.LogWarning($"FeatureSO '{featureName}' not found for {id}");
                }
            }

            // ChapterIndex 처리 (마지막 열)
            int chapterIndex = 0;
            if (values.Length > 2)
            {
                string chapterStr = values[values.Length - 1].Trim();
                if (!string.IsNullOrEmpty(chapterStr) && int.TryParse(chapterStr, out int parsed))
                {
                    chapterIndex = parsed;
                }
            }
            student.chapterIndex = chapterIndex;

            // 에셋 저장
            string assetPath = $"{studentSavePath}/{id}.asset";
            AssetDatabase.CreateAsset(student, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("학생 데이터 가져오기 완료!");
    }
}
