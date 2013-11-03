using UnityEngine;

public class CreepSpreader : MonoBehaviour
{
    private const int _widht = 3;
    private const int _height = 3;
    public CreepCell[,] creep = new CreepCell[_widht, _height];
    public Transform CellTransform;

    // Use this for initialization
    void Start()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _widht; x++)
            {
                Vector3 pos = new Vector3(x, 0, y) * 1.0f;
                Instantiate(CellTransform, pos, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
