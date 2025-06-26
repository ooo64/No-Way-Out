using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;
using System.Collections;


//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse http://www.brechtos.com



[CustomPropertyDrawer (typeof(DirectionSelectorAttribute))]
public class DirectionSelectorPropertyDrawer : PropertyDrawer
{

	string[] directions = new string[] {
		"devant",
		"haut",
		"droite"
	};

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		if (property.propertyType == SerializedPropertyType.String) {
			EditorGUI.BeginProperty (position, label, property);

			var attrib = this.attribute as DirectionSelectorAttribute;

			if (attrib.UseDefaultTagFieldDrawer) {
				property.stringValue = EditorGUI.TagField (position, label, property.stringValue);
			} else {
				//generate the directionList + custom tags
				List<string> directionList = new List<string> ();
				directionList.AddRange (directions);


				string propertyString = property.stringValue;
				int index = -1;
				if (propertyString == "") {
					//The scene is empty
					index = 0;
				} else {
					//check if there is an entry that matches the entry and get the index
					for (int i = 0; i < directionList.Count; i++) {
						if (directionList [i] == propertyString) {
							index = i;
							break;
						}
					}
				}

				//Draw the popup box with the current selected index
				index = EditorGUI.Popup (position, label.text, index, directionList.ToArray ());

				//Adjust the actual string value of the property based on the selection

				if (index >= 0) {
					property.stringValue = directionList [index];
				} else {
					property.stringValue = "";
				}
			}

			EditorGUI.EndProperty ();
		} else {
			EditorGUI.PropertyField (position, property, label);
		}
	}
}