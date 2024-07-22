using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;
    private float _value;

    public void IncrementCylindervolume(float value) 
    {
        _value += value ;
        if (_value > 1)
        {
            float leftvalue = _value - 1;
            int cylinderCunt = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCunt - 1) - 0.25f , transform.localPosition.z);
            transform.localScale = new Vector3(0.5f,transform.localScale.y,0.5f);
            PlayerController.Current.createCylinder(leftvalue );
        }
        else if (_value < 0)
        {
            //silindiri yok et
            PlayerController.Current.DestroyCyliner(this);
        }
        else 
        {
            //Silindiri g�ncelle  
            int cylinderCunt = PlayerController.Current.cylinders.Count;

            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCunt - 1) - 0.25f * _value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f* _value, transform.localScale.y, 0.5f* _value);
        }

    }

}
