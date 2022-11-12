using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
    public GameObject loader;
    public void Disable()
    {
        Destroy(gameObject);
    }

    public void ChangeScene()
    {
        loader.SetActive(true);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}