using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class PlaySound : MonoBehaviour
{
    public string soundName;

    void Awake()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AudioManager.instance.Play(soundName, false));
    }
}
