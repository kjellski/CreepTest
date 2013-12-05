using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class CreepSpreader : MonoBehaviour
{
    private const int _widht = 30;
    private const int _height = 30;
    public IFillable[,] creep = new CreepCell[_widht, _height];
    public CreepSpread spread;

    // Use this for initialization
    void Start()
    {
        // create grid
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _widht; x++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                GameObject go = (GameObject)Instantiate(Resources.Load("CreepCell"), pos, Quaternion.identity);
                creep[x, z] = go.GetComponent<CreepCell>();
                go.transform.parent = transform;
            }
        }
        // make it spreadable
        spread = new CreepSpread(_height, _widht, creep);
    }

    // Update is called once per frame
    void Update()
    {
        Spread();
    }

    private void Spread()
    {
        spread.SpreadFrom(Random.Range(0, _widht), Random.Range(0, _height), Random.Range(1, 5), Random.Range(0.1f,0.3f));
        //SmoothlyFill(Random.Range(0, _widht), Random.Range(0, _height), 0.1f);
        //spread.SpreadFrom(_widht / 2, _height / 2, 3, 0.1f);
    }

    private IEnumerator SmoothlyFill(int x, int z, float targetFilling)
    {
        var actualFilling = creep[x, z].FillingHeight;
        while (Mathf.Abs(actualFilling - targetFilling) > 0.1)
        {
            creep[x, z].FillingHeight = Mathf.MoveTowards(actualFilling, targetFilling, 0.1f);
            yield return null;
        }
    }
}
