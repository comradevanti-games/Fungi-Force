
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using TeamShrimp.GGJ23.Runtime.Networking;


[CustomEditor(typeof(TestNetCommandSender))]
public class TestNetCommandSenderEditor : Editor {
   
    // some declaration missing??
   
    override public void  OnInspectorGUI () {
        TestNetCommandSender colliderCreator = (TestNetCommandSender)target;
        
        if(GUILayout.Button("ConnInit")) {
            colliderCreator.SendDummyConnectionInitCommand();
        }
        if(GUILayout.Button("Place")) {
            colliderCreator.SendDummyPlaceCommand();
        }
        if(GUILayout.Button("World Init")) {
            colliderCreator.SendDummyWorldInitCommand();
        }
        if(GUILayout.Button("Ready")) {
            colliderCreator.SendDummyReadyCommand();
        }
        if(GUILayout.Button("Cut")) {
            colliderCreator.SendDummyCutCommand();
        }
        DrawDefaultInspector();
    }
}
#endif