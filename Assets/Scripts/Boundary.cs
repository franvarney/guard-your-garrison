using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {

    public int killedCount = 0;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "EnemyThrown" && other.gameObject.GetComponent<EnemyController>().wasThrown) {
            ++killedCount;
        }
    }
}
