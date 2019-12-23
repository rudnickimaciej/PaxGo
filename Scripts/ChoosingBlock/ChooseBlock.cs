using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBlock : MonoBehaviour
{

    [Header("Positions")]
    public Vector2 NorthPos;
    public Vector2 SouthPos;
    public Vector2 WestPos;
    public Vector2 EastPos;

    new RectTransform transform;
    Vector2 pos;
    Vector3 scale;

    Animator anim;

    [Header("Blocks")]
    public List<ChooseBlockButton> Buttons;

    void Awake()
    {
        transform = GetComponent<RectTransform>();
        pos = GetComponent<RectTransform>().anchoredPosition;
        scale = transform.localScale;
        anim = GetComponent<Animator>();

    }

    public void Set(Entity e)
    {
        SetButtonsInfo(e);

        switch (e.direction)
        {
            case Direction.North:
                transform.anchoredPosition = NorthPos;
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                return;

            case Direction.South:
                transform.anchoredPosition = SouthPos;
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(scale.x, scale.y * -1, scale.z);
                return;

            case Direction.West:
                transform.anchoredPosition = WestPos;
                transform.eulerAngles = new Vector3(0, 0, -90);
                transform.localScale = new Vector3(scale.x, scale.y * -1, scale.z);
                return;

            case Direction.East:
                transform.anchoredPosition = EastPos;
                transform.eulerAngles = new Vector3(0, 0, -90);
                transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                return;
        }
    }

    public void SetButtonsInfo(Entity e)
    {
        foreach (ChooseBlockButton b in Buttons)
        {
            b.Block.direction = e.direction;
            b.Block.x = e.x;
            b.Block.y = e.y;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
