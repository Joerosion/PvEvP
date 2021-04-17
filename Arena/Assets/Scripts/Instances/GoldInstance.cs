using UnityEngine;
public class GoldInstance : EntityInstance
{
    [SerializeField]
    private Color Bronze;
    [SerializeField]
    private Color Silver;
    [SerializeField]
    private Color Gold;


    public int amount = 1;

    public void Start()
    {
        if (amount == 1)
        {
            GetComponent<SpriteRenderer>().color = Bronze;
        }

        if(amount == 5)
        {
            GetComponent<SpriteRenderer>().color = Silver;
        }

        if(amount == 10)
        {
            GetComponent<SpriteRenderer>().color = Gold;
        }
    }
}
