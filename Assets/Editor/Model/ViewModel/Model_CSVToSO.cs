using System;
using UnityEngine;

namespace Echo.Editor
{
	[Serializable]
	public class Model_CSVToSO
	{
		public TextAsset csvfileAsset;
		public int configIndex;
	}

	public class CharacterPhysicsConfigData
	{
		public string characterType;
		public float gravity;
		public float groundGravity;
		public float rotationFactorPerFrame;
		public float maxJumpHeight;
		public float maxJumpTime;
	}
}
