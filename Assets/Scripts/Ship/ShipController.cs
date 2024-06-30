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

public class ShipController : MonoBehaviour
{
	[Header("船体信息")] 
	public ShipInfo shipInfo;
	private HighlightEffect highlightEffect;

	[Header("方向操作")] 
	public KeyCode PositiveForwardKey = KeyCode.W;
	public KeyCode NegativeForwardKey = KeyCode.S;
	public KeyCode PositiveTurnKey = KeyCode.D;
	public KeyCode NegativeTurnKey = KeyCode.A;
	
	
	[Serializable]
	public struct Velocity
	{
		public string Name;
		public float Speed;
	}
	
	[Header("引擎")] 
	[SerializeField] private Velocity[] ForwardVelocities = new Velocity[]
	{
		new Velocity { Name = "FullBack", Speed = -10.0f },
		new Velocity { Name = "Back", Speed = -5.0f },
		new Velocity { Name = "Stop", Speed = 0.0f },
		new Velocity { Name = "Forward", Speed = 5.0f },
		new Velocity { Name = "FullForward", Speed = 10.0f },
	};

	public int CurrentForwardVelocityIndex = 2;

	[SerializeField] private Velocity[] TurnVelocities = new Velocity[]
	{
		new Velocity { Name = "FullLeft", Speed = -5.0f },
		new Velocity { Name = "Left", Speed = -3.0f },
		new Velocity { Name = "Center", Speed = 0.0f },
		new Velocity { Name = "Right", Speed = 3.0f },
		new Velocity { Name = "FullRight", Speed = 5.0f },
	};

	public int CurrentTurnVelocityIndex = 2;
	
	[Header("其他设置")] 
	[SerializeField] private float ForwardAcceleration = 2.0f; // units/s^2
	[SerializeField] private float TurnAcceleration = 2.0f; // units/s^2
	[SerializeField] private float TurnSpeedAwaited = 0.0f;

	[Header("事件")] 
	public UnityEvent<float> OnChangeForwardVelocity;
	public UnityEvent<float> OnChangeTurnVelocity;

	[Header("UI")] 
	[SerializeField] private Text m_forwardSpeedText;
	[SerializeField] private Text m_turnSpeedText;
	[SerializeField] private Slider m_forwardSpeedIndicator;
	[SerializeField] private Slider m_turnSpeedIndicator;
	
	public float CurrentForwardSpeed;
	public float CurrentTurnSpeed;

	private Rigidbody m_rigidbody;
	private int m_lastForwardVelocity, m_lastTurnVelocity;
	private float m_currentForwardIndicatorValue, m_currentTurnIndicatorValue;
	
	private void Awake()
	{
		//enabled = false; //默认在prefab中关闭，通过框选激活
		highlightEffect = GetComponent<HighlightEffect>();
		
		m_rigidbody = GetComponent<Rigidbody>();
		CurrentForwardSpeed = ForwardVelocities[CurrentForwardVelocityIndex].Speed;
		CurrentTurnSpeed = TurnVelocities[CurrentTurnVelocityIndex].Speed;
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

		//RegisterShipInfo();
	}

	private void Update()
	{
		//方向控制
		if (Input.GetKeyDown(PositiveForwardKey))
		{
			MoveForward();
		}
		if (Input.GetKeyDown(NegativeForwardKey))
		{
			MoveBackward();
		}
		if (Input.GetKeyDown(PositiveTurnKey))
		{
			TurnRight();
		}
		if (Input.GetKeyDown(NegativeTurnKey))
		{
			TurnLeft();
		}

		//改变速度
		CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, ForwardVelocities[CurrentForwardVelocityIndex].Speed,
			ForwardAcceleration * Time.deltaTime);
		CurrentTurnSpeed = Mathf.MoveTowards(CurrentTurnSpeed, TurnVelocities[CurrentTurnVelocityIndex].Speed,
			TurnAcceleration * Time.deltaTime);

		//修改UI
		if (m_forwardSpeedText)
		{
			m_forwardSpeedText.text = CurrentForwardSpeed.ToString("f2");
		}

		if (m_turnSpeedText)
		{
			m_turnSpeedText.text = CurrentTurnSpeed.ToString("f2");
		}

		if (m_forwardSpeedIndicator)
		{
			if (Mathf.Abs(m_currentForwardIndicatorValue - m_forwardSpeedIndicator.value) > Mathf.Epsilon)
			{
				m_forwardSpeedIndicator.value = m_currentForwardIndicatorValue;
			}
		}

		if (m_turnSpeedIndicator)
		{
			if (Mathf.Abs(m_currentTurnIndicatorValue - m_turnSpeedIndicator.value) > Mathf.Epsilon)
			{
				m_turnSpeedIndicator.value = m_currentTurnIndicatorValue;
			}
		}
	}

	private void FixedUpdate()
	{
		if (Mathf.Abs(CurrentForwardSpeed) >= TurnSpeedAwaited)
		{
			transform.rotation *= Quaternion.AngleAxis(CurrentTurnSpeed * Time.fixedDeltaTime, Vector3.up);
		}

		m_rigidbody.velocity = transform.forward * CurrentForwardSpeed;
	}
	

	public void MoveForward()
	{
		//Slider滑动一格
		if (CurrentForwardVelocityIndex < ForwardVelocities.Length - 1)
			m_currentForwardIndicatorValue += 1.0f / (ForwardVelocities.Length - 1);

		//m_lastForwardVelocity = m_currentForwardVelocity;
		//当前速度索引+1
		CurrentForwardVelocityIndex = Math.Min(CurrentForwardVelocityIndex + 1, ForwardVelocities.Length - 1);
		
		//广播事件
		//保留两位小数
		OnChangeForwardVelocity.Invoke(ForwardVelocities[CurrentForwardVelocityIndex].Speed);

	}

	public void MoveBackward()
	{
		if (CurrentForwardVelocityIndex > 0)
			m_currentForwardIndicatorValue -= 1.0f / (ForwardVelocities.Length - 1);

		//m_lastForwardVelocity = m_currentForwardVelocity;
		CurrentForwardVelocityIndex = Math.Max(CurrentForwardVelocityIndex - 1, 0);
		OnChangeForwardVelocity.Invoke(ForwardVelocities[CurrentForwardVelocityIndex].Speed);
	}

	public void TurnRight()
	{
		if (CurrentTurnVelocityIndex < TurnVelocities.Length - 1)
			m_currentTurnIndicatorValue += 1.0f / (TurnVelocities.Length - 1);

		//m_lastTurnVelocity = m_currentTurnVelocity;
		CurrentTurnVelocityIndex = Math.Min(CurrentTurnVelocityIndex + 1, TurnVelocities.Length - 1);
		OnChangeTurnVelocity.Invoke(TurnVelocities[CurrentTurnVelocityIndex].Speed);
	}

	public void TurnLeft()
	{
		if (CurrentTurnVelocityIndex > 0)
			m_currentTurnIndicatorValue -= 1.0f / (TurnVelocities.Length - 1);

		//m_lastTurnVelocity = m_currentTurnVelocity;
		CurrentTurnVelocityIndex = Math.Max(CurrentTurnVelocityIndex - 1, 0);
		OnChangeTurnVelocity.Invoke(TurnVelocities[CurrentTurnVelocityIndex].Speed);
	}

	public void StopShip()
	{
		CurrentForwardVelocityIndex = 2;
		CurrentTurnVelocityIndex = 2;
		m_currentForwardIndicatorValue = 0.0f;
		m_currentTurnIndicatorValue = 0.0f;
		OnChangeForwardVelocity.Invoke(ForwardVelocities[CurrentForwardVelocityIndex].Speed);
		OnChangeTurnVelocity.Invoke(TurnVelocities[CurrentTurnVelocityIndex].Speed);
	}
	
	//注册船只信息
	private void RegisterShipInfo()
	{
		shipInfo.ID = Random.Range(0, 100000); //BUG:ID注册顺序
		
		
		//随机一个半透明色
		shipInfo.HighlightColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.5f);
		highlightEffect.outlineColor = shipInfo.HighlightColor;
		
		ShipManager.Instance.ShipInfos.Add(shipInfo);
	}
	
	//初始化船只信息
	public void InitShipInfo(int ID)
	{
		//信息收集
		shipInfo.ID = ID; 
		shipInfo.HighlightColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.5f);//随机一个半透明色
		highlightEffect.outlineColor = shipInfo.HighlightColor;
		
		//注册到Manager
		ShipManager.Instance.RegisterShipInfo(this);
	}
}
