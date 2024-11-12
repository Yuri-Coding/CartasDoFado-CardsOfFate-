using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DeckManagerMP))]
public class DeckManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeckManagerMP deckManager = (DeckManagerMP)target;
        if (GUILayout.Button("Draw Next Card")){
            HandManagerMP handManager = FindObjectOfType<HandManagerMP>();
            if (handManager != null){
                deckManager.DrawCard(handManager);
            }
        }
    }
}
#endif