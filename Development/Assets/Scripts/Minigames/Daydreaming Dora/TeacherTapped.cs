using UnityEngine;
using System.Collections;

public class TeacherTapped : MonoBehaviour
{

		DoraManager doraManager;
		[System.NonSerialized]
		public bool isShowing = false;
		public Collider teacherCollider;
		public bool gameStarted = false;

		void Awake ()
		{
				teacherCollider = collider;
				SetDoraManager ();
				if (doraManager == null) {
						Invoke ("SetDoraManager", 1);
						Debug.LogWarning ("Dora Manager was not found");
				}
		}

		void SetDoraManager ()
		{
			doraManager = GameObject.FindObjectOfType (typeof(DoraManager)) as DoraManager;
		}
	
		void OnClick ()
		{
			if (enabled) {
		
						if (!isShowing) {
								isShowing = true;
								doraManager.showHint ();
						} else if (!gameStarted){
								doraManager.TeacherTap ();
								gameStarted = true;
						}
				}
		}
}
