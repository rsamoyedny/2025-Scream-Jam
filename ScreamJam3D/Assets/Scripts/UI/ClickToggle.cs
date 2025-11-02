using UnityEngine;
using UnityEngine.UI;

public class ClickToggle : MonoBehaviour
{
    public Toggle toggle;
    private bool startSwap;
    [SerializeField] private string associatedPref = "ClickRotate";
    void Awake()
    {
        startSwap = true;
        toggle.isOn = PlayerPrefs.GetInt(associatedPref, 0) == 1;
        startSwap = false;
    }

    public void Toggle()
    {
        if (!startSwap)
        {
            PlayerPrefs.SetInt(associatedPref, PlayerPrefs.GetInt(associatedPref, 0) == 0 ? 1 : 0);
            PlayerPrefs.Save();
            toggle.isOn = PlayerPrefs.GetInt("ClickRotate", 0) == 1;
        }
    }
}
