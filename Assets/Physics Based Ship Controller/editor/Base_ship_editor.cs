using UnityEngine;
using UnityEditor;

namespace Vbertz.PBSC
{

	[CustomEditor(typeof(PhysicsBasedShipController))]
	public class ShipController_editor : Editor
	{
		public Texture2D Header; // Asegúrate de que esta imagen esté en tu carpeta de Resources
		public Texture2D Sur_Type_img;
		public Texture2D Sur_Elements_img;


		void OnEnable()
		{
			Header = Resources.Load<Texture2D>("header");
			Sur_Type_img = Resources.Load<Texture2D>("Sur_Type_img");
			Sur_Elements_img = Resources.Load<Texture2D>("Sur_Elements_img");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label(Header, GUILayout.MaxHeight(150));

			EditorGUILayout.LabelField("预览", EditorStyles.boldLabel);

			GUILayout.Label(Sur_Type_img, GUILayout.MaxHeight(100));

			GUILayout.Label(Sur_Elements_img, GUILayout.MaxHeight(100));

			DrawDefaultInspector();

		}
	}
}