using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadManufacturing()
    {
        SceneManager.LoadScene("Scene2");
    }
    public void Hospital()
    {
        SceneManager.LoadScene("Scene1");
    }
}
