using System.Collections.Generic;
using UnityEngine;

namespace Hover
{
	[RequireComponent(typeof(Rigidbody))]
	public class HoverController : MonoBehaviour
	{
		[SerializeField]
		private float m_accelerationForward;

		[SerializeField]
		private float m_accelerationBackward;

		[SerializeField]
		private float m_turnStrength;

		[SerializeField]
		private float m_hoverForce;

		[SerializeField]
		private float m_hoverHeight;

		[SerializeField]
		private List<Transform> m_hoverPoints;

		[SerializeField]
		private float m_downwardPull;

		private Rigidbody m_rigidbody;
		private float m_inputDeadzone = 0.1f;
		private float m_currentThrust;
		private float m_currentTurn;
		private int m_layerMask;

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_layerMask = ~(1 << LayerMask.NameToLayer("Vehicle"));
		}

		private void Update()
		{
			m_currentThrust = 0.0f;
			float accelerationAxis = Input.GetAxis("Vertical");
			if (accelerationAxis > m_inputDeadzone)
			{
				m_currentThrust = accelerationAxis * m_accelerationForward;
			}
			else if (accelerationAxis < m_inputDeadzone)
			{
				m_currentThrust = accelerationAxis * m_accelerationBackward;
			}

			m_currentTurn = 0.0f;
			float turnAxis = Input.GetAxis("Horizontal");
			if (Mathf.Abs(turnAxis) > m_inputDeadzone)
			{
				m_currentTurn = turnAxis;
			}
		}

		private void FixedUpdate()
		{
			RaycastHit hit;
			for (int i = 0; i < m_hoverPoints.Count; i++)
			{
				Transform hoverPoint = m_hoverPoints[i];
				if (Physics.Raycast(hoverPoint.transform.position, Vector3.down, out hit, m_hoverHeight, m_layerMask))
				{
					Debug.DrawRay(hoverPoint.transform.position, Vector3.down * m_hoverHeight, Color.red);
					m_rigidbody.AddForceAtPosition(Vector3.up * m_hoverForce * (1.0f - (hit.distance / m_hoverHeight)), hoverPoint.transform.position);
					continue;
				}
				Debug.DrawRay(hoverPoint.transform.position, Vector3.down * m_hoverHeight, Color.blue);

				//if (transform.position.y > hoverPoint.transform.position.y)
				//{
				//	m_rigidbody.AddForceAtPosition(hoverPoint.transform.up * m_hoverForce * 0.2f, hoverPoint.transform.position);
				//	continue;
				//}

				m_rigidbody.AddForceAtPosition(hoverPoint.transform.up * -m_hoverForce * m_downwardPull, hoverPoint.transform.position);
			}

			if (Mathf.Abs(m_currentThrust) > 0.0f)
			{
				m_rigidbody.AddForce(transform.forward * m_currentThrust);
			}

			if (m_currentTurn > 0 || m_currentTurn < 0)
			{
				m_rigidbody.AddRelativeTorque(Vector3.up * m_currentTurn * m_turnStrength);
			}
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < m_hoverPoints.Count; i++)
			{
				if (m_hoverPoints[i] == null)
				{
					continue;
				}
				Gizmos.DrawSphere(m_hoverPoints[i].position, 0.1f);
			}
		}
	}
}