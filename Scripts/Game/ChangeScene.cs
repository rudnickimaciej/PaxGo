using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ChangeScene : MonoBehaviour
{
    public int index;
    public ScenesManager sman;
    UnityEngine.UI.Button b;

    void Awake()
    {
        sman = FindObjectOfType<ScenesManager>();
        b = GetComponent<UnityEngine.UI.Button>();
        b.onClick.AddListener(() => sman.LoadScene(index));
    }
}
