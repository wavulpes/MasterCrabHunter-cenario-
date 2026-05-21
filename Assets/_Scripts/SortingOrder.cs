using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    private SpriteRenderer sr;
    public int offset = 100;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10) + offset;
    }
}