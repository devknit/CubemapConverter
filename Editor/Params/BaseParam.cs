
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace CubemapConverter
{
	[System.Serializable]
	public class BaseParam
	{
		public BaseParam( bool opened)
		{
			defaultOpened = opened;
		}
		public virtual void OnEnable( Window window)
		{
			handle = window;
			if( enabled == null)
			{
				enabled = new AnimBool( defaultOpened);
				enabled.valueChanged.AddListener( window.Repaint);
			}
		}
		public virtual void OnDisable()
		{
		}
		public void Record( string label)
		{
			handle.Record( label);
		}
		public virtual void OnGUI( ConvertType convertType)
		{
		}
		protected void OnPUI( string caption, System.Action callback)
		{
			EditorGUILayout.BeginVertical( GUI.skin.box);
			{
				EditorGUILayout.BeginHorizontal();
				{
					bool target = EditorGUILayout.Foldout( enabled.target, caption);
					if( enabled.target.Equals( target) == false)
					{
						Record( "Change Foldout");
						enabled.target = target;
					}
				}
				EditorGUILayout.EndHorizontal();
				
				if( EditorGUILayout.BeginFadeGroup( enabled.faded) != false)
				{
					++EditorGUI.indentLevel;
					callback?.Invoke();
					--EditorGUI.indentLevel;
				}
				EditorGUILayout.EndFadeGroup();
			}
			EditorGUILayout.EndVertical();
		}
		
		[SerializeField]
		bool defaultOpened;
		[SerializeField]
		AnimBool enabled;
		[System.NonSerialized]
		Window handle;
	}
}
