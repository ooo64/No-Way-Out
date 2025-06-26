using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TagSelectorAttribute : PropertyAttribute
{
	public bool UseDefaultTagFieldDrawer = false;
}