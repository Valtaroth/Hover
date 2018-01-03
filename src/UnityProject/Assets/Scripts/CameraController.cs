using UnityEngine;

namespace Valtaroth.Hover
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private Vector3 m_positionOffset;

		[SerializeField]
		private float m_positionLerpSpeed;

		[SerializeField]
		private Vector3 m_rotationOffset;

		[SerializeField]
		private float m_rotationLerpSpeed;

		private void Update()
		{
			Vector3 targetPosition = m_target.position + m_target.right * m_positionOffset.x + Vector3.up * m_positionOffset.y + m_target.forward * m_positionOffset.z;
			Vector3 position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * m_positionLerpSpeed);
			transform.position = position;

			targetPosition = m_target.position + m_target.right * m_rotationOffset.x + Vector3.up * m_rotationOffset.y + m_target.forward * m_rotationOffset.z;
			Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position).normalized, Vector3.up);
			Quaternion rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationLerpSpeed);
			transform.rotation = rotation;
		}
	}
}