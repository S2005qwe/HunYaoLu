using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;
    public GameObject MainCanvas;

    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }
}