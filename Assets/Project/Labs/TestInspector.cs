using System;
using UnityEngine;
namespace Labs
{
	[Serializable]
	public class Animal : ScriptableObject
	{
		public string m_name;
	}

	[Serializable]
	public class Cat : Animal
	{
		public string miao;
	}

	[Serializable]
	public class Dog : Animal
	{
		public string wong;
	}

	[ExecuteAlways]
	public class TestInspector : MonoBehaviour
	{
		[SerializeReference]
		public Animal animal;
		void Awake()
		{
			animal = ScriptableObject.CreateInstance<Animal>();
		}
	}
}