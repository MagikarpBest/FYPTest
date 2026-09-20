using UnityEngine;

public class Destroyable : MonoBehaviour,IDestroyable
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
