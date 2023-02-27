using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionTeleporter : MonoBehaviour
{
    [SerializeField]
    private Dimension baseDimension;
    
    public Vector2 getBaseDimension(){
        return baseDimension.transform.position;
    }

    public Vector2 getBaseDimensionEntrance(){
        return baseDimension.transform.position;
    }
}
