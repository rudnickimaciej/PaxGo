using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{

    public GameObject Panel;
    public Animator[] Stars;

    void Awake()
    {

    }

    public void ShowPanel(int Stars)
    {
        StartCoroutine(ShowPanelCoroutine(Stars));
    }

    IEnumerator ShowPanelCoroutine(int Stars)
    {
        Panel.SetActive(true);
        yield return StartCoroutine(AnimateStars(Stars));
        Panel.GetComponent<Animator>().SetTrigger("buttons");
    }

    IEnumerator AnimateStars(int starsCount)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < starsCount; i++)
        {
            Stars[i].SetTrigger("star");
            AudioManager.instance.Play("star" + i, false, 1 + (0.25f * i));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
