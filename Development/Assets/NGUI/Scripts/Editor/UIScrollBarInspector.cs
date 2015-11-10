//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIScrollBar))]
public class UIScrollBarInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		EditorGUIUtility.LookLikeControls(80f);
		UIScrollBar sb = target as UIScrollBar;

		NGUIEditorTools.DrawSeparator();

		float val = EditorGUILayout.Slider("Value", sb.scrollValue, 0f, 1f);
		float size = EditorGUILayout.Slider("Size", sb.barSize, 0f, 1f);
		float alpha = EditorGUILayout.Slider("Alpha", sb.alpha, 0f, 1f);
		Camera cameraToUpdate = (Camera)EditorGUILayout.ObjectField("Camera to Update", sb.cameraToUpdate, typeof(Camera), true);
		//UITable npcContainer = (UITable)EditorGUILayout.ObjectField ("NPC Container", sb.npcContainer, typeof(UITable), true);
		GameObject npcContainer = (GameObject)EditorGUILayout.ObjectField ("NPC Container", sb.npcContainer, typeof(GameObject), true);
		
		NGUIEditorTools.DrawSeparator();

		UISprite bg = (UISprite)EditorGUILayout.ObjectField("Background", sb.background, typeof(UISprite), true);
		UISprite fg = (UISprite)EditorGUILayout.ObjectField("Foreground", sb.foreground, typeof(UISprite), true);
		UIScrollBar.Direction dir = (UIScrollBar.Direction)EditorGUILayout.EnumPopup("Direction", sb.direction);
		bool inv = EditorGUILayout.Toggle("Inverted", sb.inverted);

		if (sb.scrollValue != val ||
			sb.barSize != size ||
			sb.background != bg ||
			sb.foreground != fg ||
			sb.direction != dir ||
			sb.inverted != inv ||
			sb.alpha != alpha ||
			sb.cameraToUpdate != cameraToUpdate ||
			sb.npcContainer != npcContainer)
		{
			NGUIEditorTools.RegisterUndo("Scroll Bar Change", sb);
			sb.scrollValue = val;
			sb.barSize = size;
			sb.inverted = inv;
			sb.background = bg;
			sb.foreground = fg;
			sb.direction = dir;
			sb.alpha = alpha;
			sb.cameraToUpdate = cameraToUpdate;
			sb.npcContainer = npcContainer;
			UnityEditor.EditorUtility.SetDirty(sb);
		}
	}
}