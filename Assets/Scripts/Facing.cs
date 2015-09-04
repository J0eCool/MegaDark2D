using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Facing : JComponent {
	[SerializeField] private bool facingRight = false;

	public bool FacingRight {
		get { return facingRight; }
		set { facingRight = value; }
	}

	public int Dir { get { return facingRight ? 1 : -1; } }
}
