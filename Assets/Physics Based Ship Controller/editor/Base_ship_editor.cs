using UnityEngine;
using UnityEditor;

namespace Vbertz.PBSC
{

	[CustomEditor(typeof(PhysicsBasedShipController))]
	public class ShipController_editor : Editor
	{
		public Texture2D Header; // Asegúrate de que esta imagen esté en tu carpeta de Resources
		public Texture2D Sub_Type_img; // Asegúrate de que esta imagen esté en tu carpeta de Resources
		public Texture2D Sur_Type_img;
		public Texture2D Sub_Elements_img;
		public Texture2D Sur_Elements_img;



		void OnEnable()
		{
			Header = Resources.Load<Texture2D>("header");
			Sub_Type_img = Resources.Load<Texture2D>("Sub_Type_img");
			Sur_Type_img = Resources.Load<Texture2D>("Sur_Type_img");
			Sur_Elements_img = Resources.Load<Texture2D>("Sur_Elements_img");
			Sub_Elements_img = Resources.Load<Texture2D>("Sub_Elements_img");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label(Header, GUILayout.MaxHeight(150));


			EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
			//GUILayout.Label(Sub_Type_img, GUILayout.MaxHeight(100));

			PhysicsBasedShipController shipController = (PhysicsBasedShipController)target;
			shipController.IsSubmarine = EditorGUILayout.Toggle("Is Submarine", shipController.IsSubmarine);
			shipController.IsSurfaceShip = EditorGUILayout.Toggle("Is Surface Ship", shipController.IsSurfaceShip);

			if (shipController.IsSubmarine)
			{
				GUILayout.Label(Sub_Type_img, GUILayout.MaxHeight(100));
				ChangeToDeep();
			}
			else
			{
				GUILayout.Label(Sur_Type_img, GUILayout.MaxHeight(100));
				ChangeToSurface();
			}


			void ChangeToDeep()
			{
				shipController.IsSurfaceShip = false;
				shipController.IsSubmarine = true;
			}

			void ChangeToSurface()
			{
				shipController.IsSubmarine = false;
				shipController.IsSurfaceShip = true;
			}

			EditorGUILayout.LabelField("Elements", EditorStyles.boldLabel);
			if (shipController.IsSubmarine)
			{
				GUILayout.Label(Sub_Elements_img, GUILayout.MaxHeight(100));
				ChangeToDeep();
			}
			else
			{
				GUILayout.Label(Sur_Elements_img, GUILayout.MaxHeight(100));
				ChangeToSurface();
			}

			DrawDefaultInspector();

		}
	}
}