using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.IO;
using System.Collections;


//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse http://www.brechtos.com



[CustomPropertyDrawer(typeof(KeySelectorAttribute))]
public class KeySelectorPropertyDrawer : PropertyDrawer
{

	string[] keys = new string[]{
		"a",
		"b",
		"c",
		"d",
		"e",
		"f",
		"g",
		"h",
		"i",
		"j",
		"k",
		"l",
		"m",
		"n",
		"o",
		"p",
		"q",
		"r",
		"s",
		"t",
		"u",
		"v",
		"w",
		"x",
		"y",
		"z",
		"mouse 0",
		"mouse 1",
		"mouse 2",
		"space",
		"f1",
		"f2",
		"f3",
		"f4",
		"f5",
		"f6",
		"f7",
		"f8",
		"f9",
		"f10",
		"f11",
		"f12",
		"f13",
		"f14",
		"f15",
		"0",
		"1",
		"2",
		"3",
		"4",
		"5",
		"6",
		"7",
		"8",
		"9",
		"!",
		"\"",
		"#",
		"$",
		"&",
		"'",
		"(",
		")",
		"*",
		"+",
		",",
		"-",
		".",
		"/",
		":",
		";",
		"<",
		"=",
		">",
		"?",
		"@",
		"[",
		"\\",
		"]",
		"^",
		"_",
		"`",
		"numlock",
		"caps lock",
		"scroll lock",
		"right shift",
		"left shift",
		"right ctrl",
		"left ctrl",
		"right alt",
		"left alt",
		"backspace",
		"delete",
		"tab",
		"clear",
		"return",
		"pause",
		"escape",
		"up",
		"down",
		"right",
		"left",
		"insert",
		"home",
		"end",
		"page up",
		"page down"
	};

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.String)
		{
			EditorGUI.BeginProperty(position, label, property);

			var attrib = this.attribute as KeySelectorAttribute;

			if (attrib.UseDefaultTagFieldDrawer)
			{
				property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
			}
			else
			{
				//generate the keyList + custom tags
				List<string> keyList = new List<string>();
				keyList.AddRange(keys);


				string propertyString = property.stringValue;
				int index = -1;
				if(propertyString =="")
				{
					//The scene is empty
					index = 0;
				}
				else
				{
					//check if there is an entry that matches the entry and get the index
					for (int i = 0; i < keyList.Count; i++)
					{
						if (keyList[i] == propertyString)
						{
							index = i;
							break;
						}
					}
				}

				//Draw the popup box with the current selected index
				index = EditorGUI.Popup(position, label.text, index, keyList.ToArray());

				//Adjust the actual string value of the property based on the selection

				if (index >= 0)
				{
					property.stringValue = keyList[index];
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