using UnityEngine;
namespace Labs
{
	public class TestInstantiate : MonoBehaviour
	{
		public GameObject prefab;
		void Start()
		{
			Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
		}
	}
}