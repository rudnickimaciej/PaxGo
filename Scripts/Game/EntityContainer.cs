using UnityEngine;
using UnityEngine.UI;
public class EntityContainer : MonoBehaviour
{
    public Image image;

    public new RectTransform transform;
    [SerializeField] Entity entity;
    public Entity Entity
    {
        set
        {
            entity = value;
            image.sprite = Resources.Load<Sprite>(entity.slug);
            gameObject.name = entity.slug + "_" + entity.x + " " + entity.y;
        }
        get
        {
            return entity;
        }
    }
    protected virtual void Awake()
    {

        transform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            Controller controller = (Controller)FindObjectOfType(typeof(Controller));
            btn.onClick.AddListener(delegate { controller.ShowBlockPanel(this.entity, GetComponent<Button>()); });
        }
    }



    protected virtual void Start()
    {

    }

    ///<summary>
    /// Returns real position of EntityContainer
    ///</summary>

    public Vector2 GetPosition()
    {
        return transform.anchoredPosition;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.anchoredPosition = pos;
    }
}
