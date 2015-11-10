//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UISpriteAnimations.
/// </summary>

[CustomEditor(typeof(UITextureAnimation))]
public class UITextureAnimationInspector : Editor
{
	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.DrawSeparator();
		EditorGUIUtility.LookLikeControls(80f);
		UITextureAnimation anim = target as UITextureAnimation;
		
		serializedObject.Update();
       	
       	EditorGUIUtility.LookLikeInspector();
       	SerializedProperty textures = serializedObject.FindProperty ("textures");
       	EditorGUI.BeginChangeCheck();
       	EditorGUILayout.PropertyField(textures, true);
       	if(EditorGUI.EndChangeCheck())
         	serializedObject.ApplyModifiedProperties();
       	EditorGUIUtility.LookLikeControls();
		
		NGUIEditorTools.DrawSeparator();
		
		int fps = EditorGUILayout.IntField("Framerate", anim.framesPerSecond);
		fps = Mathf.Clamp(fps, 0, 60);

		if (anim.framesPerSecond != fps)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.framesPerSecond = fps;
			EditorUtility.SetDirty(anim);
		}

		string namePrefix = EditorGUILayout.TextField("Name Prefix", (anim.namePrefix != null) ? anim.namePrefix : "");

		if (anim.namePrefix != namePrefix)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.namePrefix = namePrefix;
			EditorUtility.SetDirty(anim);
		}

		bool loop = EditorGUILayout.Toggle("Loop", anim.loop);

		if (anim.loop != loop)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.loop = loop;
			EditorUtility.SetDirty(anim);
		}
		
		bool ping_Pong = EditorGUILayout.Toggle("Ping Pong", anim.mPing_Pong);

		if (anim.mPing_Pong != ping_Pong)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.mPing_Pong = ping_Pong;
			EditorUtility.SetDirty(anim);
		}
		
		bool playOnStart = EditorGUILayout.Toggle("Play on start", anim.playOnStart);

		if (anim.playOnStart != playOnStart)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.playOnStart = playOnStart;
			EditorUtility.SetDirty(anim);
		}
	}
}