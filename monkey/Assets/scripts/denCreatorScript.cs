using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenCreator : MonoBehaviour
{
    public int denWidth, denLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;


    // Start is called before the first frame update
    void Start()
    {
        createDen();
    }

    private void createDen()
    {
        DenGenerator generator = new DenGenerator(denWidth, denLength);
        var listOFRooms = generator.CalculateRooms(maxIterations, roomLengthMin, roomWidthMin);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
