using UnityEngine;
using System.Collections;

public class InitCannon : MonoBehaviour
{

	public Component barrel;
	public float barrelStartAngle;

	// Use this for initialization
	void Start()
	{
		barrel.transform.rotation = Quaternion.Euler(barrelStartAngle, 0, 0);
	}

}
