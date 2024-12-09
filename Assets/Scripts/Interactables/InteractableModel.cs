using System;
using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 특정한 <see cref="InteractableBase"/>의 외형 정보를 관리한다. <br/> 
	/// 여기서 외형이란, 해당 상호작용 개체의 메쉬(Renderer + Filter)와 콜라이더 이다.
	/// </summary>
	/// <remarks>
	/// 이 클래스는 스스로는 아무 것도 하지 않고, 오직 <see cref="InteractableBase"/>의 명령만을 수행한다.
	/// </remarks>
	[DisallowMultipleComponent]
	public class InteractableModel : MonoBehaviour
	{
		[Serializable]
		public struct MeshElement
		{
			public MeshRenderer Renderer { get; private set; }
			public MeshFilter Filter { get; private set; }

			public MeshElement(MeshRenderer renderer, MeshFilter filter)
			{
				Renderer = renderer;
				Filter = filter;
			}
		}

		[Tooltip("이 게임 오브젝트의 계층 구조에서 외형 표현을 위한 렌더러가 부착된 모든 오브젝트를 여기에 할당한다. 구체적으로는 메쉬 필터와 메쉬 렌더러가 함께 존재하는 모든 오브젝트를 여기에 할당한다.")]
		[SerializeField] private List<GameObject> m_rendererAttachedObjects = new();

		[Tooltip("이 게임 오브젝트의 계층 구조에서 충돌 판정을 위한 콜라이더가 부착된 모든 오브젝트를 여기에 할당")]
		[SerializeField] private List<GameObject> m_colliderAttachedObjects = new();

		private List<MeshElement> m_meshElements;
		private List<Collider> m_colliders;

		public IReadOnlyList<MeshElement> MeshDatas => m_meshElements;
		public IReadOnlyList<Collider> Colliders => m_colliders;

		public void InitializeColliders()
		{
			// 모든 콜라이더를 찾아서 저장

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

		public void InitializeMeshElements()
		{
			// 모든 메쉬 필터와 렌더러를 찾아서 저장

			m_meshElements = new();

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

				m_meshElements.Add(new MeshElement(renderer, filter));
			}
		}
	}
}