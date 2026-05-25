using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class TransformData
{
    public string sceneName;
    public string objectName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool useDefaultPosition;
}

[System.Serializable]
public class TransformDataCollection
{
    public List<TransformData> transforms = new List<TransformData>();
}

public class TransformSaver : MonoBehaviour
{
    public string targetTag = "SaveTransform";
    public string specialSceneName = "xingzuo";
    public Vector3 specialScenePosition = Vector3.zero;
    public Quaternion specialSceneRotation = Quaternion.identity;
    public KeyCode quitKey = KeyCode.Escape;

    private string savePath;
    private TransformDataCollection savedTransforms;
    private bool isQuitting = false;
    private bool isLoadingScene = false;
    private bool isJsonUpdateDisabled = false; // 新增标志变量

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SavedTransforms.json");

        LoadTransforms();

        if (FindObjectsOfType<TransformSaver>().Length > 1)
        {
           
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnApplicationQuitting;
        }
    }

    private void Update()
    {
        if (isQuitting || isLoadingScene) return;

        if (!isJsonUpdateDisabled)
        {
            UpdateTransforms();
        }

        if (Input.GetKeyDown(quitKey))
        {
            QuitApplication();
        }
    }

    public void QuitApplication()
    {
        if (isQuitting) return;

        isQuitting = true;
        DeleteSaveFile();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted successfully.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting save file: {e.Message}");
        }
    }

    private void UpdateTransforms()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);


        foreach (var obj in targetObjects)
        {
            TransformData existingData = savedTransforms.transforms.Find(t =>
                t.objectName == obj.name && t.sceneName == currentScene);

            if (existingData != null)
            {
                if (!isLoadingScene)
                {
                    existingData.position = obj.transform.position;
                    existingData.rotation = obj.transform.rotation;
                    existingData.scale = obj.transform.localScale;
                }
                existingData.useDefaultPosition = ShouldUseDefaultPosition(obj);
              
            }
            else
            {
                TransformData newData = new TransformData
                {
                    sceneName = currentScene,
                    objectName = obj.name,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    scale = obj.transform.localScale,
                    useDefaultPosition = ShouldUseDefaultPosition(obj)
                };
                savedTransforms.transforms.Add(newData);
                
        }

        if (!isJsonUpdateDisabled)
        {
            SaveTransforms();
        }
    }
    }
    private bool ShouldUseDefaultPosition(GameObject obj)
    {
        bool isSpecialScene = SceneManager.GetActiveScene().name == specialSceneName;
       ;
        return isSpecialScene;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RestoreTransformsAfterFrame());

        // 判断是否跳转到了指定场景
        if (scene.name == specialSceneName)
        {
            // 修改 SavePosition 中 Transform 数据为指定数据
            foreach (var data in savedTransforms.transforms)
            {
                if (data.sceneName == specialSceneName)
                {
                    data.position = specialScenePosition;
                    data.rotation = specialSceneRotation;
                   
            }

            if (!isJsonUpdateDisabled)
            {
                SaveTransforms();
            }
        }
    }
    }
    private IEnumerator RestoreTransformsAfterFrame()
    {
        isLoadingScene = true;
        yield return null;

        string currentScene = SceneManager.GetActiveScene().name;
        

        List<TransformData> sceneTransforms = savedTransforms.transforms.FindAll(t => t.sceneName == currentScene);

        // 如果是xingzuo场景，只更新JSON数据，不改变实际对象位置
        if (currentScene == specialSceneName)
        {
            foreach (var data in sceneTransforms)
            {
                if (data.useDefaultPosition)
                {
                    data.position = specialScenePosition;
                    data.rotation = specialSceneRotation;
                    
                }
            }

            if (!isJsonUpdateDisabled)
            {
                SaveTransforms();
            }
        }

        // 应用实际位置（不强制使用默认位置）
        foreach (var data in sceneTransforms)
        {
            GameObject obj = GameObject.Find(data.objectName);
            if (obj != null && obj.CompareTag(targetTag))
            {
                obj.transform.position = data.position;
                obj.transform.rotation = data.rotation;
                obj.transform.localScale = data.scale;
   
            }
            else
            {
                Debug.LogWarning($"Object {data.objectName} not found or missing tag in scene {currentScene}");
            }
        }

        isLoadingScene = false;
    }

    public void LoadSpecialScene()
    {
       
        // 先保存当前场景数据
        if (!isJsonUpdateDisabled)
        {
            UpdateTransforms();
        }

        // 加载场景前预更新xingzuo场景的数据
        foreach (var data in savedTransforms.transforms)
        {
            if (data.sceneName == specialSceneName && data.useDefaultPosition)
            {
                data.position = specialScenePosition;
                data.rotation = specialSceneRotation;
                
            }
        }

        if (!isJsonUpdateDisabled)
        {
            SaveTransforms();
        }
        SceneManager.LoadScene(specialSceneName);
    }

    private void SaveTransforms()
    {
        if (isJsonUpdateDisabled) return;

        try
        {
            string json = JsonUtility.ToJson(savedTransforms, true);
            File.WriteAllText(savePath, json);
            //Debug.Log($"Saved transforms to JSON. Total entries: {savedTransforms.transforms.Count}");
            //Debug.Log(json); // 输出完整的 JSON 内容
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving transforms: {e.Message}");
        }
    }

    private void LoadTransforms()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                savedTransforms = JsonUtility.FromJson<TransformDataCollection>(json);
                Debug.Log($"Loaded transforms from JSON. Total entries: {savedTransforms.transforms.Count}");
                Debug.Log(json); // 输出加载的 JSON 内容
            }
            else
            {
                savedTransforms = new TransformDataCollection();
         
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading transforms: {e.Message}");
            savedTransforms = new TransformDataCollection();
        }
    }

    private void OnApplicationQuitting()
    {
        if (!isQuitting)
        {
           
            DeleteSaveFile();
        }
    }

    private void OnDestroy()
    {
       
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= OnApplicationQuitting;
    }

    public void SetSpecialScenePosition(string objectName, Vector3 position, Quaternion rotation)
    {
        var data = savedTransforms.transforms.Find(t =>
            t.objectName == objectName && t.sceneName == specialSceneName);

        if (data != null)
        {
            data.position = position;
            data.rotation = rotation;
            data.useDefaultPosition = false;
            Debug.Log($"Updated custom position for {objectName} in {specialSceneName}");
        }
        else
        {
            TransformData newData = new TransformData
            {
                sceneName = specialSceneName,
                objectName = objectName,
                position = position,
                rotation = rotation,
                scale = Vector3.one,
                useDefaultPosition = false
            };
            savedTransforms.transforms.Add(newData);
            Debug.Log($"Added new custom position for {objectName} in {specialSceneName}");
        }

        if (!isJsonUpdateDisabled)
        {
            SaveTransforms();
        }
    }

    // 禁用 JSON 数据更新
    public void DisableJsonUpdate()
    {
        isJsonUpdateDisabled = true;
        Debug.Log("JSON data update disabled.");
    }

    // 解除禁用 JSON 数据更新
    public void EnableJsonUpdate()
    {
        isJsonUpdateDisabled = false;
        Debug.Log("JSON data update enabled.");
    }
    public void Dele()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted successfully.");
        }
    }
}