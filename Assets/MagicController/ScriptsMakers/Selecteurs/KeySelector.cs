using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class KeySelectorAttribute : PropertyAttribute
{
	public bool UseDefaultTagFieldDrawer = false;
}