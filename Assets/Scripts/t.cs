using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //StartCoroutine(x());
    }

    IEnumerator x() {
        yield return new WaitForSeconds(5f);
        Debug.Log(GameObject.FindObjectOfType<Grid>().NodeFromGrid(transform.position).worldPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
