using UnityEngine;
using System.Collections;

public class InitCannon : MonoBehaviour
{

	public Component barrel;
	public float barrelStartAngle;

	void Awake()
	{
		barrel.transform.rotation = Quaternion.Euler(barrelStartAngle, 0, 0);
	}

}
