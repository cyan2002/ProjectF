using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private bool inRange = false;

    void OnEnable()
    {
        PlayerInput.HandleE += TrySleep;
    }

    void OnDisable()
    {
        PlayerInput.HandleE -= TrySleep;
    }

    private void TrySleep()
    {
        Debug.Log("!");

        if (inRange)
        {
            //allow player to sleep if the correct time
            //after 8 PM before 5 AM is the correct time
            if (Clock.Instance.timeOfDay >= 20)
            {
                Clock.Instance.SetClock(6, Clock.Instance.dayNumber + 1);
            }
            else if (Clock.Instance.timeOfDay <= 5)
            {
                Clock.Instance.SetClock(6, Clock.Instance.dayNumber);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
}
