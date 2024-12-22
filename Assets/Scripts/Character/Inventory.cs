using BM.Interactables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
//using echo17.EndlessBook;
using UnityEngine.Animations;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace BM
{
	public class Inventory : MonoBehaviour
	{
		[SerializeField] InputReaderSO m_InputReader;
		[SerializeField] GameObject m_Book;
		[SerializeField] EndlessBook m_Book_Page;
		[SerializeField] private List<InteractableSO> m_inventory = new();
		[SerializeField] private InteractableSO m_Equipped = null;
		[SerializeField] Camera m_Inventory_ViewCamera;
		[SerializeField] RectTransform m_Inventory_RenderTexture;

		// Start is called before the first frame update
		void Start()
		{
			//InteractableSO ItemSO = new InteractableSO();
			//ItemSO.
			return;
		}

		// Update is called once per frame
		void Update()
		{
			return;
		}

		void Render()
		{
			return;
		}

		private void OnEnable()
		{
			m_InputReader.Open_InventoryPerformed += OnOpen_InventoryPerformed;
			m_InputReader.Open_InventoryCanceled += OnOpen_InventoryCanceled;

			m_InputReader.Close_InventoryPerformed += OnClose_InventoryPerformed;
			m_InputReader.Close_InventoryCanceled += OnClose_InventoryCanceled;

			m_InputReader.Next_PagePerformed += OnNext_PagePerformed;
			m_InputReader.Next_PageCanceled += OnNext_PageCanceled;

			m_InputReader.Previous_PagePerformed += OnPrevious_PagePerformed;
			m_InputReader.Previous_PageCanceled += OnPrevious_PageCanceled;

			m_InputReader.Click_LeftPerformed += OnClick_LeftPerformed;
			m_InputReader.Click_LeftCanceled += OnClick_LeftCanceled;

			m_InputReader.Click_RightPerformed += OnClick_RightPerformed;
			m_InputReader.Click_RightCanceled += OnClick_RightCanceled;



			return;
		}

		private void OnDisable()
		{
			m_InputReader.Open_InventoryPerformed -= OnOpen_InventoryPerformed;
			m_InputReader.Open_InventoryCanceled -= OnOpen_InventoryCanceled;

			m_InputReader.Close_InventoryPerformed -= OnClose_InventoryPerformed;
			m_InputReader.Close_InventoryCanceled -= OnClose_InventoryCanceled;

			m_InputReader.Next_PagePerformed -= OnNext_PagePerformed;
			m_InputReader.Next_PageCanceled -= OnNext_PageCanceled;

			m_InputReader.Previous_PagePerformed -= OnPrevious_PagePerformed;
			m_InputReader.Previous_PageCanceled -= OnPrevious_PageCanceled;

			m_InputReader.Click_LeftPerformed -= OnClick_LeftPerformed;
			m_InputReader.Click_LeftCanceled -= OnClick_LeftCanceled;

			m_InputReader.Click_RightPerformed -= OnClick_RightPerformed;
			m_InputReader.Click_RightCanceled -= OnClick_RightCanceled;
			return;
		}

		private void OnOpen_InventoryPerformed()
		{
		}

		private void OnOpen_InventoryCanceled()
		{
			Debug.Log("OnOpen_InventoryPerformed");

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			m_InputReader.SetActiveActionMap(InputReaderSO.EActionMap.Gameplay_UI);
			m_Book.SetActive(true);
			//m_Book.GetComponent<Animation>().Play();


			return;
		}


		private void OnNext_PagePerformed()
		{
			//m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);
			if (m_Book_Page.CurrentState == EndlessBook.StateEnum.ClosedFront)
			{
				m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);
				return;
			}

			m_Book_Page.TurnToPage(m_Book_Page.CurrentPageNumber + 20, EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime, 1.2f);
			return;
		}

		private void OnNext_PageCanceled()
		{
			return;
		}

		private void OnPrevious_PagePerformed()
		{
			if (m_Book_Page.CurrentState == EndlessBook.StateEnum.OpenMiddle && m_Book_Page.CurrentPageNumber == 1)
			{
				m_Book_Page.SetState(EndlessBook.StateEnum.ClosedFront);
				return;
			}

			m_Book_Page.TurnToPage(m_Book_Page.CurrentPageNumber - 20, EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime, 1.2f);
			return;
		}

		private void OnPrevious_PageCanceled()
		{
			return;
		}

		private void OnClose_InventoryPerformed()
		{
			return;
		}

		private void OnClose_InventoryCanceled()
		{
			Debug.Log("OnClose_InventoryPerformed");
			//m_Book_Page.SetState(EndlessBook.StateEnum.ClosedFront);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			m_InputReader.SetActiveActionMap(InputReaderSO.EActionMap.Gameplay);
			m_Book.SetActive(false);
			return;
		}

		private void OnClick_LeftPerformed()
		{
			return;
		}

		private void OnClick_LeftCanceled()
		{
			OnEquipment();
			return;
		}

		private void OnClick_RightPerformed()
		{
			return;
		}

		private void OnClick_RightCanceled()
		{
			return;
		}
		public void OnEquipment()
		{

			//Ray rRay = Raycast();
			//RaycastHit rhHit;
			////Debug.DrawRay(rRay);
			
			//if (Physics.Raycast(rRay, out rhHit, Mathf.Infinity/*, raycastLayerMask*/))
			//{
			//	string stName = rhHit.transform.gameObject.name;
			//	stName.Replace("Slot", "");
			//	int iCount = int.Parse(stName);
			//	if (Get_ItemCount() != 0 && Get_ItemCount() >= iCount - 1)
			//	{
			//		m_Equipped = m_inventory[iCount - 1];
			//	}
			//	else
			//	{
			//		Debug.LogWarning("!비어있는 슬롯!");
			//	}
			//}
			//else
			//{
			//	Debug.LogWarning("!슬롯 없음!");
			//}
			
		}

		private void Raycast()
		{
			//Vector2 vMousePos = Input.mousePosition;
			//Vector3 vViewportPoint = m_Inventory_ViewCamera.ScreenToViewportPoint(vMousePos);

			//Vector2 hitPointNormalized = new Vector2(vViewportPoint.x, vViewportPoint.y);


		}

		public void Add_Item(InteractableSO ItemSO)
		{
			if(Get_ItemCount() < 12)
			{
				m_inventory.Add(ItemSO);
			}
			else
			{
				Debug.LogWarning("!아이템 슬롯 부족!");
			}
		}


		// 인벤토리 로드(파일)
		void Load_Inventory()
		{
			//파일을 어떻게 만들까?
		}
		// 인벤토리 저장(파일)
		void Save_Inventory()
		{
			//파일을 어떻게 만들까?
		}

		//void Add_Item(STItemInfo stItemInfo)
		//{
		//	stItemList.Add(stItemInfo);
		//}

		//void Delete_Item(int iIndex)
		//{
		//	stItemList.RemoveAt(iIndex);
		//}

		//void Delete_Item(STItemInfo stItemInfo)
		//{
		//	//stItemList.RemoveAt(iIndex);
		//}



		void OpenBookEvent()
		{
			//m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);
		}

		void CloseBookEvent()
		{

		}

		

		public void PutIn(InteractableSO interactable)
		{
			m_inventory.Add(interactable);
		}

		//private List<STItemInfo> stItemList;
		//public List<STItemInfo> Get_ItemList() { return stItemList; }
		//public STItemInfo Get_ItemInfo(int iIndex) { return stItemList[iIndex]; }
		public int Get_ItemCount() { return m_inventory.Count; }

		//private InputReader.EActionMap m_ActionMap = InputReader.EActionMap.Gameplay;


		//공용으로 다룰수 있는 구조체들을 가지고 있는 스크립트를 짜주는게 좋지 않을까?
		//C++이랑 너무 많이 다르다...

	}
}
