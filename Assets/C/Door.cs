using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Trigger[] triger;
    public bool isTrigger;
    public bool isAni;
    public float CurrentHp;
    public float MaxHp;
    private void Update()
    {
        if (triger.All(t => t.keyCount > 0))
        {
            isTrigger = true;
            DoorO();
        }

    }
    public void DoorO()
    {
        if (!isAni)
        {

          

            gameObject.GetComponent<Animator>().SetBool("isDoor", true);

            isAni = true;
        }
    }

}
