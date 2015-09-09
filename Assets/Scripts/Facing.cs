using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Facing : JComponent {
	[SerializeField] private bool facingRight = false;

    [StartComponent] private tk2dSprite sprite = null;

	public bool FacingRight {
		get { return facingRight; }
		set { facingRight = value; }
	}

	public int Dir { get { return facingRight ? 1 : -1; } }

    void Update() {
        sprite.FlipX = !facingRight;
    }
}
