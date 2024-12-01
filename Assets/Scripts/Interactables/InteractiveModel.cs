using System;
using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	public class InteractiveModel : MonoBehaviour
	{
		[Serializable]
		public struct MeshData
		{
			public MeshRenderer Renderer { get; private set; }
			public MeshFilter Filter { get; private set; }

			public MeshData(MeshRenderer renderer, MeshFilter filter)
			{
				Renderer = renderer;
				Filter = filter;
			}
		}

		[Tooltip("이 게임 오브젝트의 계층 구조에서 외형 표현을 위한 렌더러가 부착된 모든 오브젝트를 여기에 할당한다. 구체적으로는 메쉬 필터와 메쉬 렌더러가 함께 존재하는 모든 오브젝트를 여기에 할당한다.")]
		[SerializeField] private List<GameObject> m_rendererAttachedObjects = new();

		[Tooltip("이 게임 오브젝트의 계층 구조에서 충돌 판정을 위한 콜라이더가 부착된 모든 오브젝트를 여기에 할당")]
		[SerializeField] private List<GameObject> m_colliderAttachedObjects = new();

		private List<MeshData> m_meshDatas;
		private List<Collider> m_colliders;

		public IReadOnlyList<MeshData> MeshDatas => m_meshDatas;
		public IReadOnlyList<Collider> Colliders => m_colliders;

		private void Awake()
		{
			m_meshDatas = new();

			foreach (GameObject renderingObject in m_rendererAttachedObjects)
			{
				if (!renderingObject.TryGetComponent<MeshRenderer>(out var renderer))
				{
					continue;
				}

				if (!renderingObject.TryGetComponent<MeshFilter>(out var filter))
				{
					continue;
				}

				m_meshDatas.Add(new MeshData(renderer, filter));
			}

			m_colliders = new();

			foreach (GameObject collidingObject in m_colliderAttachedObjects)
			{
				if (!collidingObject.TryGetComponent<Collider>(out var collider))
				{
					continue;
				}

				m_colliders.Add(collider);
			}
		}
	}
}