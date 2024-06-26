using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vbertz.PBSC
{
	public class Physics_Based_Ship_Controller : MonoBehaviour
	{
		[HideInInspector] [Header("Type")] public bool IsSubmarine;
		[HideInInspector] public bool IsSurfaceShip;

		[Serializable]
		public struct Velocity
		{
			public string Name;
			public float Speed;

		}

		[Serializable]
		public struct ShipTransform
		{
			public string Name;
			public float Height;
			public Vector3 position;
		}

		[Header("State")] public bool InSurface;
		public bool BelowSurface;

		[Serializable]
		public class StringEvent : UnityEvent<string>
		{
		}

		[Header("Control Keys")] [Header("Setup Submarine")] [SerializeField]
		public KeyCode m_EmergeKey = KeyCode.E;

		public KeyCode EmergeKey
		{
			get { return m_EmergeKey; }
		}

		[SerializeField] public KeyCode m_DiveKey = KeyCode.Q;

		public KeyCode DiveKey
		{
			get { return m_DiveKey; }
		}

		[Header("For Surface and below")] [SerializeField]
		public KeyCode m_positiveForwardKey = KeyCode.W;

		public KeyCode PositiveForwardKey
		{
			get { return m_positiveForwardKey; }
		}

		[SerializeField] public KeyCode m_negativeForwardKey = KeyCode.S;

		public KeyCode NegativeForwardKey
		{
			get { return m_negativeForwardKey; }
		}

		[SerializeField] public KeyCode m_positiveTurnKey = KeyCode.D;

		public KeyCode PositiveTurnKey
		{
			get { return m_positiveTurnKey; }
		}

		[SerializeField] public KeyCode m_negativeTurnKey = KeyCode.A;

		public KeyCode NegativeTurnKey
		{
			get { return m_negativeTurnKey; }
		}


		[Header("ENGINE")] [SerializeField] private Velocity[] m_forwardVelocities = new Velocity[]
		{
			new Velocity { Name = "FullBack", Speed = -10.0f },
			new Velocity { Name = "Back", Speed = -5.0f },
			new Velocity { Name = "Stop", Speed = 0.0f },
			new Velocity { Name = "Forward", Speed = 5.0f },
			new Velocity { Name = "FullForward", Speed = 10.0f },
		};

		[SerializeField] private int m_currentForwardVelocity = 2;

		[SerializeField] private Velocity[] m_turnVelocities = new Velocity[]
		{
			new Velocity { Name = "FullLeft", Speed = -5.0f },
			new Velocity { Name = "Left", Speed = -3.0f },
			new Velocity { Name = "Center", Speed = 0.0f },
			new Velocity { Name = "Right", Speed = 3.0f },
			new Velocity { Name = "FullRight", Speed = 5.0f },
		};

		[SerializeField] private int m_currentTurnVelocity = 2;

		[Header("DIVE")] [SerializeField] private ShipTransform[] m_ShipHeigt = new ShipTransform[]

		{
			new ShipTransform { Name = "Surface", Height = 0.0f },
			new ShipTransform { Name = "Atack_Deep", Height = -5.0f },
			new ShipTransform { Name = "Dive_Soft", Height = -8.0f },
			new ShipTransform { Name = "Escape_Deep", Height = -15.0f },



		};

		//[SerializeField] private int currentHeightIndex = 0;
		[SerializeField] public float targetHeight;
		[SerializeField] private float heightChangeSpeed = 2.0f;

		private float m_shipHeightMin;
		private float m_shipHeightMax;
		public int m_shipHeightIndex;


		[Header("Others Settings")] [SerializeField]
		private float m_forwardAcceleration = 2.0f; // units/s^2

		[SerializeField] private float m_turnAcceleration = 2.0f; // units/s^2

		[SerializeField] private float m_turnSpeedAwaited = 0.0f;

		[Header("Events")] [SerializeField] private StringEvent m_onChangeForwardVelocity;

		[SerializeField] private StringEvent m_onChangeTurnVelocity;

		[Header("UI")] [SerializeField] private Text m_forwardSpeedText;
		[SerializeField] private Text m_turnSpeedText;

		[SerializeField] private Slider m_forwardSpeedIndicator;

		[SerializeField] private Slider m_turnSpeedIndicator;

		public float CurrentSpeed
		{
			get { return m_speed; }
			set { m_speed = value; }
		}

		public float CurrentTurnSpeed
		{
			get { return m_turnSpeed; }
			set { m_turnSpeed = value; }
		}

		public int CurrentForwardVelocity
		{
			get { return m_currentForwardVelocity; }
			set { m_currentForwardVelocity = value; }
		}

		public int CurrentTurnVelocity
		{
			get { return m_currentTurnVelocity; }
			set { m_currentTurnVelocity = value; }
		}

		private float m_speed;
		private float m_turnSpeed;

		private Rigidbody m_rigidbody;
		private float m_minForward, m_maxForward;
		private float m_minTurn, m_maxTurn;
		private int m_lastForwardVelocity, m_lastTurnVelocity;
		private float m_currentForwardIndicatorValue, m_currentTurnIndicatorValue;



		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_speed = m_forwardVelocities[m_currentForwardVelocity].Speed;
			m_turnSpeed = m_turnVelocities[m_currentTurnVelocity].Speed;

			m_minForward = m_forwardVelocities.Min(x => x.Speed);
			m_maxForward = m_forwardVelocities.Max(x => x.Speed);

			m_minTurn = m_turnVelocities.Min(x => x.Speed);
			m_maxTurn = m_turnVelocities.Max(x => x.Speed);

			m_shipHeightMin = m_ShipHeigt.Min(x => x.Height);
			m_shipHeightMax = m_ShipHeigt.Max(x => x.Height);



		}

		private void Start()
		{
			if (m_forwardSpeedIndicator)
			{
				m_currentForwardIndicatorValue = m_forwardSpeedIndicator.value;
			}

			if (m_turnSpeedIndicator)
			{
				m_currentTurnIndicatorValue = m_turnSpeedIndicator.value;
			}

		}

		public void GoUp()
		{
			if (m_shipHeightIndex < m_ShipHeigt.Length - 1)
				m_shipHeightIndex = Math.Min(m_shipHeightIndex + 1, m_ShipHeigt.Length - 1);
		}

		public void GoDown()
		{
			if (m_shipHeightIndex > 0)
				m_shipHeightIndex = Math.Max(m_shipHeightIndex - 1, 0);
		}


		public void MoveForward()
		{
			if (m_currentForwardVelocity < m_forwardVelocities.Length - 1)
				m_currentForwardIndicatorValue += 1.0f / (m_forwardVelocities.Length - 1);

			//m_lastForwardVelocity = m_currentForwardVelocity;
			m_currentForwardVelocity = Math.Min(m_currentForwardVelocity + 1, m_forwardVelocities.Length - 1);
			m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity].Speed.ToString("f2"));
		}

		public void MoveBackward()
		{

			if (m_currentForwardVelocity > 0)
				m_currentForwardIndicatorValue -= 1.0f / (m_forwardVelocities.Length - 1);

			//m_lastForwardVelocity = m_currentForwardVelocity;
			m_currentForwardVelocity = Math.Max(m_currentForwardVelocity - 1, 0);
			m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity].Speed.ToString("f2"));
		}

		public void TurnRight()
		{
			if (m_currentTurnVelocity < m_turnVelocities.Length - 1)
				m_currentTurnIndicatorValue += 1.0f / (m_turnVelocities.Length - 1);

			//m_lastTurnVelocity = m_currentTurnVelocity;
			m_currentTurnVelocity = Math.Min(m_currentTurnVelocity + 1, m_turnVelocities.Length - 1);
			m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity].Speed.ToString("f2"));
		}

		public void TurnLeft()
		{
			if (m_currentTurnVelocity > 0)
				m_currentTurnIndicatorValue -= 1.0f / (m_turnVelocities.Length - 1);

			//m_lastTurnVelocity = m_currentTurnVelocity;
			m_currentTurnVelocity = Math.Max(m_currentTurnVelocity - 1, 0);
			m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity].Speed.ToString("f2"));
		}

		public void StopShip()
		{
			m_rigidbody.velocity = Vector3.zero;
		}

		private void Update()
		{
			if (Input.GetKeyDown(EmergeKey) && IsSubmarine == true)
			{
				GoUp();
			}

			if (Input.GetKeyDown(DiveKey) && IsSubmarine == true)
			{
				GoDown();
			}

			float targetY = m_ShipHeigt[m_shipHeightIndex].Height;
			float newY = Mathf.Lerp(transform.position.y, targetY, heightChangeSpeed * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, newY, transform.position.z);


			if (Input.GetKeyDown(m_positiveForwardKey))
			{
				if (m_currentForwardVelocity < m_forwardVelocities.Length - 1)
					m_currentForwardIndicatorValue += 1.0f / (m_forwardVelocities.Length - 1);

				//m_lastForwardVelocity = m_currentForwardVelocity;
				m_currentForwardVelocity = Math.Min(m_currentForwardVelocity + 1, m_forwardVelocities.Length - 1);
				m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity].Speed.ToString("f2"));
			}

			if (Input.GetKeyDown(m_negativeForwardKey))
			{
				if (m_currentForwardVelocity > 0)
					m_currentForwardIndicatorValue -= 1.0f / (m_forwardVelocities.Length - 1);

				//m_lastForwardVelocity = m_currentForwardVelocity;
				m_currentForwardVelocity = Math.Max(m_currentForwardVelocity - 1, 0);
				m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity].Speed.ToString("f2"));
			}

			if (Input.GetKeyDown(m_positiveTurnKey))
			{
				if (m_currentTurnVelocity < m_turnVelocities.Length - 1)
					m_currentTurnIndicatorValue += 1.0f / (m_turnVelocities.Length - 1);

				//m_lastTurnVelocity = m_currentTurnVelocity;
				m_currentTurnVelocity = Math.Min(m_currentTurnVelocity + 1, m_turnVelocities.Length - 1);
				m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity].Speed.ToString("f2"));
			}

			if (Input.GetKeyDown(m_negativeTurnKey))
			{
				if (m_currentTurnVelocity > 0)
					m_currentTurnIndicatorValue -= 1.0f / (m_turnVelocities.Length - 1);

				//m_lastTurnVelocity = m_currentTurnVelocity;
				m_currentTurnVelocity = Math.Max(m_currentTurnVelocity - 1, 0);
				m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity].Speed.ToString("f2"));
			}

			m_speed = Mathf.MoveTowards(m_speed, m_forwardVelocities[m_currentForwardVelocity].Speed,
				m_forwardAcceleration * Time.deltaTime);
			m_turnSpeed = Mathf.MoveTowards(m_turnSpeed, m_turnVelocities[m_currentTurnVelocity].Speed,
				m_turnAcceleration * Time.deltaTime);

			if (m_forwardSpeedText)
			{
				m_forwardSpeedText.text = m_speed.ToString("f2");
			}

			if (m_turnSpeedText)
			{
				m_turnSpeedText.text = m_turnSpeed.ToString("f2");
			}

			if (m_forwardSpeedIndicator)
			{
				/*var delta = (m_currentForwardIndicatorValue - m_forwardSpeedIndicator.value) / 
				    ((m_forwardVelocities[m_currentForwardVelocity].Speed - m_speed) / m_forwardAcceleration);*/

				if (Mathf.Abs(m_currentForwardIndicatorValue - m_forwardSpeedIndicator.value) > Mathf.Epsilon)
				{
					//m_forwardSpeedIndicator.value = Mathf.MoveTowards(m_forwardSpeedIndicator.value, m_currentForwardIndicatorValue, Mathf.Abs(delta) * Time.deltaTime);
					m_forwardSpeedIndicator.value = m_currentForwardIndicatorValue;
				}
			}

			if (m_turnSpeedIndicator)
			{
				/*var delta = (m_currentTurnIndicatorValue - m_turnSpeedIndicator.value) / 
				    ((m_turnVelocities[m_currentTurnVelocity].Speed - m_speed) / m_turnAcceleration);*/

				if (Mathf.Abs(m_currentTurnIndicatorValue - m_turnSpeedIndicator.value) > Mathf.Epsilon)
				{
					//m_turnSpeedIndicator.value = Mathf.MoveTowards(m_turnSpeedIndicator.value, m_currentTurnIndicatorValue, Mathf.Abs(delta) * Time.deltaTime);
					m_turnSpeedIndicator.value = m_currentTurnIndicatorValue;
				}
			}
		}

		private void FixedUpdate()
		{
			if (Mathf.Abs(m_speed) >= m_turnSpeedAwaited)
			{
				transform.rotation *= Quaternion.AngleAxis(m_turnSpeed * Time.fixedDeltaTime, Vector3.up);
			}

			m_rigidbody.velocity = transform.forward * m_speed;


		}

	}
}
