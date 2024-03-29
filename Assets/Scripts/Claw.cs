﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour {

    void Start() {
        Destroy(this.gameObject, 0.1f);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == 8) {
            Vector2 dir = ((Vector2)collision.gameObject.transform.position - (Vector2)transform.position).normalized;
            Player Target = collision.GetComponent<Player>();
            Target.KnockBack(dir);
        }
    }
}
