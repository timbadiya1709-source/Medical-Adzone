using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadManufacturing(string Manufacturing)
    {
        SceneManager.LoadScene(Manufacturing);
    }
    public void Hospital(string Hospital)
    {
        SceneManager.LoadScene(Hospital);
    }
}
