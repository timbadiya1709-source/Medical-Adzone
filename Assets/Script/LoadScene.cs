using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

public void LoadScene2()
{
    if (SceneStateManagerRoot.Instance != null)
        SceneStateManagerRoot.Instance.LoadScene("Scene2");
    else
        SceneManager.LoadScene("Scene2");
}   
public void LoadScene1()
{
    if (SceneStateManagerRoot.Instance != null)
        SceneStateManagerRoot.Instance.LoadScene("Scene1");
    else
        SceneManager.LoadScene("Scene1");
}   

}
