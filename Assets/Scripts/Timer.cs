using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {

        GetComponent<TextMeshProUGUI>().text = System.DateTime.Now.ToString();
    }
}
