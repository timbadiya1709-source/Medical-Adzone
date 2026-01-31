using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Scene2()
    {SceneStateManagerRoot.Instance.SaveAndLoad("Scene2");


    }
public void Scene1() { SceneStateManagerRoot.Instance.SaveAndLoad("Scene1"); }
}
