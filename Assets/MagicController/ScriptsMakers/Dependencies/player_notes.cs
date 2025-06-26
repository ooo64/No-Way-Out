using UnityEngine;

public class player_notes : MonoBehaviour {
#if UNITY_EDITOR
	[TextAreaAttribute(1, 10)]
	public string comment = "Ce personnage peut nager si, et seulement si, l'objet dans lequel on veut nager possède: un rigidbody, un collider trigger, un tag flotte et un layer Water";
#endif
}
