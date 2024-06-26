
using UnityEngine;
using UnityEditor;

namespace Vbertz.PBSC
{
[CustomEditor(typeof(BoatTiltController))]
public class Boat_Tilt_editor : Editor
{
	
	public Texture2D Header;
	
    // Start is called before the first frame update
    void OnEnable()
    {
        Header = Resources.Load<Texture2D>("Tilt_header");
    }
	
	public override void OnInspectorGUI()
    {
        GUILayout.Label(Header, GUILayout.MaxHeight(150));
		DrawDefaultInspector();
   
    }
}
}