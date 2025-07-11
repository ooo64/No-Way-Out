﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;
using System.Collections;


//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse http://www.brechtos.com

[CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
public class SceneSelectorPropertyDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.String)
		{
			EditorGUI.BeginProperty(position, label, property);

			var attrib = this.attribute as SceneSelectorAttribute;

			if (attrib.UseDefaultTagFieldDrawer)
			{
				property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
			}
			else
			{
				//generate the sceneList + custom tags
				List<string> sceneList = new List<string>();
				sceneList.Add("<NoScene>");

				foreach (EditorBuildSettingsScene sc in EditorBuildSettings.scenes){
					sceneList.Add(Path.GetFileNameWithoutExtension(sc.path));

				}

				string propertyString = property.stringValue;
				int index = -1;
				if(propertyString =="")
				{
					//The scene is empty
					index = 0; //first index is the special <notag> entry
				}
				else
				{
					//check if there is an entry that matches the entry and get the index
					//we skip index 0 as that is a special custom case
					for (int i = 1; i < sceneList.Count; i++)
					{
						if (sceneList[i] == propertyString)
						{
							index = i;
							break;
						}
					}
				}

				//Draw the popup box with the current selected index
				index = EditorGUI.Popup(position, label.text, index, sceneList.ToArray());

				//Adjust the actual string value of the property based on the selection
				if(index==0)
				{
					property.stringValue = "";
				}
				else if (index >= 1)
				{
					property.stringValue = sceneList[index];
				}
				else
				{
					property.stringValue = "";
				}
			}

			EditorGUI.EndProperty();
		}
		else
		{
			EditorGUI.PropertyField(position, property, label);
		}
	}
}