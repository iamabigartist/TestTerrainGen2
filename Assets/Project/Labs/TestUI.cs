using UnityEngine;
namespace Labs
{
	public class TestUI : MonoBehaviour
	{
		void Start()
		{
			var canvas = GetComponent<Canvas>();
			canvas.enabled = false;
		}

		void Update() {}
	}
}