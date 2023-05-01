using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditMenuScript : MonoBehaviour
{
    public void ClickBackButton()
    {
        SceneManager.LoadScene("FinalMenu", LoadSceneMode.Single);
    }
}
