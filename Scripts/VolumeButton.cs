using UnityEngine;

public class VolumeButton : UnityEngine.UI.Button
{
    VolumeSettings volSet;
    protected override void Awake()
    {
        base.Awake();
        volSet = GameObject.FindObjectOfType<VolumeSettings>();
        onClick.AddListener(() => volSet.Toggle());
    }

}
