using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBlockButton : Button
{
    Controller Controller;
    [SerializeField] public Block Block;
    protected override void Awake()
    {
        base.Awake();

        Controller = GameObject.FindObjectOfType<Controller>();
        onClick.AddListener(() => Controller.TurnMainLoop(this));

    }
}
