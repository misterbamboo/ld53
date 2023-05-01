using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIScript : MonoBehaviour
{
    public void ClickPlayButton()
    {
        SceneManager.LoadScene("FinalGame", LoadSceneMode.Single);
    }

    public void ClickCreditsButton()
    {
        SceneManager.LoadScene("FinalCredits", LoadSceneMode.Single);
    }

    public void ClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
