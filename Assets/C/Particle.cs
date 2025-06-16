using UnityEngine;

public class Particle : MonoBehaviour
{
    public float time = 0.5f;
    private void OnEnable()
    {
        Invoke("EnableOb", time);
    }

    void EnableOb()
    {
        gameObject.SetActive(false);
    }
}
