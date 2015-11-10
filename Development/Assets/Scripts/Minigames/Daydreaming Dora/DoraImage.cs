using UnityEngine;
using System.Collections;

public class DoraImage : MonoBehaviour {

	public enum DoraColor {
		RED,
		ORANGE,
		YELLOW,
		GREEN,
		BLUE,
	}

	public DoraColor color;

	public enum DoraCategory {
		beach,
		farm,
		garden,
		kitchen,
		space,
	}

	public DoraCategory category;

}
