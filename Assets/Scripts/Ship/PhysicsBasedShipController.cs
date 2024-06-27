using System;
using UnityEngine;
using System.Linq;
using Common;
using HighlightPlus;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PhysicsBasedShipController : MonoBehaviour
{
	[Header("船体信息")] 
	public ShipInfo shipInfo;
	private HighlightEffect highlightEffect;
	
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

	#region 键位
	[Header("方向操作")] 
	[SerializeField] public KeyCode m_positiveForwardKey = KeyCode.W;

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
	#endregion
	
	[Header("引擎")] 
	[SerializeField] private Velocity[] m_forwardVelocities = new Velocity[]
	{
		new Velocity { Name = "FullBack", Speed = -10.0f },
		new Velocity { Name = "Back", Speed = -5.0f },
		new Velocity { Name = "Stop", Speed = 0.0f },
		new Velocity { Name = "Forward", Speed = 5.0f },
		new Velocity { Name = "FullForward", Speed = 10.0f },
	};

	[SerializeField] private int m_currentForwardVelocity_index = 2;

	[SerializeField] private Velocity[] m_turnVelocities = new Velocity[]
	{
		new Velocity { Name = "FullLeft", Speed = -5.0f },
		new Velocity { Name = "Left", Speed = -3.0f },
		new Velocity { Name = "Center", Speed = 0.0f },
		new Velocity { Name = "Right", Speed = 3.0f },
		new Velocity { Name = "FullRight", Speed = 5.0f },
	};

	[SerializeField] private int m_currentTurnVelocity_index = 2;

	[Header("其他设置")] 
	[SerializeField] private float m_forwardAcceleration = 2.0f; // units/s^2
	[SerializeField] private float m_turnAcceleration = 2.0f; // units/s^2
	[SerializeField] private float m_turnSpeedAwaited = 0.0f;

	[Header("事件")] 
	public UnityEvent<float> m_onChangeForwardVelocity;
	public UnityEvent<float> m_onChangeTurnVelocity;

	[Header("UI")] 
	[SerializeField] private Text m_forwardSpeedText;
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

	public int CurrentForwardVelocityIndex
	{
		get { return m_currentForwardVelocity_index; }
		set { m_currentForwardVelocity_index = value; }
	}

	public int CurrentTurnVelocityIndex
	{
		get { return m_currentTurnVelocity_index; }
		set { m_currentTurnVelocity_index = value; }
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
		highlightEffect = GetComponent<HighlightEffect>();
		
		m_rigidbody = GetComponent<Rigidbody>();
		m_speed = m_forwardVelocities[m_currentForwardVelocity_index].Speed;
		m_turnSpeed = m_turnVelocities[m_currentTurnVelocity_index].Speed;

		m_minForward = m_forwardVelocities.Min(x => x.Speed);
		m_maxForward = m_forwardVelocities.Max(x => x.Speed);

		m_minTurn = m_turnVelocities.Min(x => x.Speed);
		m_maxTurn = m_turnVelocities.Max(x => x.Speed);
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

		RegisterShipInfo();
	}

	public void MoveForward()
	{
		//Slider滑动一格
		if (m_currentForwardVelocity_index < m_forwardVelocities.Length - 1)
			m_currentForwardIndicatorValue += 1.0f / (m_forwardVelocities.Length - 1);

		//m_lastForwardVelocity = m_currentForwardVelocity;
		//当前速度索引+1
		m_currentForwardVelocity_index = Math.Min(m_currentForwardVelocity_index + 1, m_forwardVelocities.Length - 1);
		
		//广播事件
		//保留两位小数
		m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity_index].Speed);

	}

	public void MoveBackward()
	{

		if (m_currentForwardVelocity_index > 0)
			m_currentForwardIndicatorValue -= 1.0f / (m_forwardVelocities.Length - 1);

		//m_lastForwardVelocity = m_currentForwardVelocity;
		m_currentForwardVelocity_index = Math.Max(m_currentForwardVelocity_index - 1, 0);
		m_onChangeForwardVelocity.Invoke(m_forwardVelocities[m_currentForwardVelocity_index].Speed);
	}

	public void TurnRight()
	{
		if (m_currentTurnVelocity_index < m_turnVelocities.Length - 1)
			m_currentTurnIndicatorValue += 1.0f / (m_turnVelocities.Length - 1);

		//m_lastTurnVelocity = m_currentTurnVelocity;
		m_currentTurnVelocity_index = Math.Min(m_currentTurnVelocity_index + 1, m_turnVelocities.Length - 1);
		m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity_index].Speed);
	}

	public void TurnLeft()
	{
		if (m_currentTurnVelocity_index > 0)
			m_currentTurnIndicatorValue -= 1.0f / (m_turnVelocities.Length - 1);

		//m_lastTurnVelocity = m_currentTurnVelocity;
		m_currentTurnVelocity_index = Math.Max(m_currentTurnVelocity_index - 1, 0);
		m_onChangeTurnVelocity.Invoke(m_turnVelocities[m_currentTurnVelocity_index].Speed);
	}

	public void StopShip()
	{
		m_rigidbody.velocity = Vector3.zero;
	}

	private void Update()
	{
		//方向控制
		if (Input.GetKeyDown(m_positiveForwardKey))
		{
			MoveForward();
		}
		if (Input.GetKeyDown(m_negativeForwardKey))
		{
			MoveBackward();
		}
		if (Input.GetKeyDown(m_positiveTurnKey))
		{
			TurnRight();
		}
		if (Input.GetKeyDown(m_negativeTurnKey))
		{
			TurnLeft();
		}

		//改变速度
		m_speed = Mathf.MoveTowards(m_speed, m_forwardVelocities[m_currentForwardVelocity_index].Speed,
			m_forwardAcceleration * Time.deltaTime);
		m_turnSpeed = Mathf.MoveTowards(m_turnSpeed, m_turnVelocities[m_currentTurnVelocity_index].Speed,
			m_turnAcceleration * Time.deltaTime);

		//修改UI
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

	//注册船只信息
	private void RegisterShipInfo()
	{
		shipInfo.ID = Random.Range(0, 100000);
		
		//随机一个半透明色
		shipInfo.HighlightColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.5f);
		highlightEffect.outlineColor = shipInfo.HighlightColor;
		
		PhysicsBasedShipManager.Instance.ShipInfos.Add(shipInfo);
	}
}
