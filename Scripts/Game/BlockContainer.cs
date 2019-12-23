using System.Collections;
using UnityEngine;

public class BlockContainer : EntityContainer
{
    Animator anim;
    public float JellyRateMin = 0f;
    public float JellyRateMax = 3f;
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }
    protected override void Start()
    {
        StartCoroutine(JellyEnum(0));
        Jelly();
    }
    void Jelly()
    {
        float time = Random.Range(JellyRateMin, JellyRateMax);
        StartCoroutine(JellyEnum(time));
    }
    IEnumerator JellyEnum(float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetTrigger("Jelly");
        yield return new WaitForSeconds(1f);
        Jelly();
    }
}
