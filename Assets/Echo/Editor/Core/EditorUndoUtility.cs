using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public static class EditorUndoUtility
	{
		/// <summary>
		/// 记录对象当前状态，用于撤销属性修改
		/// </summary>
		public static void RecordObject(Object target, string name)
		{
			if (target == null)
				return;

			Undo.RecordObject(target, name);
		}

		/// <summary>
		/// 记录对象组当前状态，用于撤销属性修改
		/// </summary>
		public static void RecordObjects(Object[] targets, string name)
		{
			if (targets == null || targets.Length == 0)
				return;

			Undo.RecordObjects(targets, name);
		}

		/// <summary>
		/// 注册创建对象撤销事件
		/// </summary>
		public static void RegisterCreateObject(Object target, string name)
		{
			if (target == null)
				return;

			Undo.RegisterCreatedObjectUndo(target, name);
		}

		/// <summary>
		/// 销毁对象撤销事件
		/// </summary>
		public static void DestoryObject(Object target)
		{
			if (target == null)
				return;

			Undo.DestroyObjectImmediate(target);
		}

		/// <summary>
		/// 添加组件撤销事件
		/// </summary>
		public static T AddComponment<T>(GameObject target)
			where T : Component
		{
			if (target == null)
				return null;

			return Undo.AddComponent<T>(target);
		}

		/// <summary>
		/// 修改Transform父节点撤销事件
		/// </summary>
		public static void SetParent(Transform target, Transform parent, string name)
		{
			if (target == null)
				return;

			Undo.SetTransformParent(target, parent, name);
		}
	}
}
