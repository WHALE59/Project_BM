using BM.Interactables;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
//using echo17.EndlessBook;
using UnityEngine.Animations;

namespace BM
{
	public class Inventory : MonoBehaviour
	{
		[SerializeField] InputReaderSO m_InputReader;
		[SerializeField] GameObject m_Book;
		[SerializeField] EndlessBook m_Book_Page;
		[SerializeField] private List<InteractableSO> m_inventory = new();
		// Start is called before the first frame update
		void Start()
		{
			stItemList = new List<STItemInfo>(new STItemInfo[12]);
			//stItemList.
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

			m_InputReader.Next_PagePerformed += OnNext_PagePerformed;
			m_InputReader.Next_PageCanceled += OnNext_PageCanceled;

			m_InputReader.Previous_PagePerformed += OnPrevious_PagePerformed;
			m_InputReader.Previous_PageCanceled += OnPrevious_PageCanceled;
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
			return;
		}

		private void OnOpen_InventoryPerformed()
		{
		}

		private void OnOpen_InventoryCanceled()
		{
			Debug.Log("OnOpen_InventoryPerformed");


			m_InputReader.SetActiveActionMap(InputReaderSO.EActionMap.Gameplay_UI);
			m_Book.SetActive(true);
			//m_Book.GetComponent<Animation>().Play();


			return;
		}


		private void OnNext_PagePerformed()
		{
			m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);
			if (m_Book_Page.CurrentState == EndlessBook.StateEnum.ClosedFront)
			{
				m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);

			}

			m_Book_Page.TurnToPage(m_Book_Page.CurrentPageNumber + 10, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1.2f);
			return;
		}

		private void OnNext_PageCanceled()
		{
			return;
		}

		private void OnPrevious_PagePerformed()
		{
			if (m_Book_Page.CurrentState == EndlessBook.StateEnum.OpenMiddle)
			{
				m_Book_Page.SetState(EndlessBook.StateEnum.ClosedFront);
			}

			m_Book_Page.TurnToPage(m_Book_Page.CurrentPageNumber - 10, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1.2f);
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
			m_InputReader.SetActiveActionMap(InputReaderSO.EActionMap.Gameplay);
			m_Book.SetActive(false);
			return;
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

		void Add_Item(STItemInfo stItemInfo)
		{
			stItemList.Add(stItemInfo);
		}

		void Delete_Item(int iIndex)
		{
			stItemList.RemoveAt(iIndex);
		}

		void Delete_Item(STItemInfo stItemInfo)
		{
			//stItemList.RemoveAt(iIndex);
		}



		void OpenBookEvent()
		{
			m_Book_Page.SetState(EndlessBook.StateEnum.OpenMiddle);
		}

		void CloseBookEvent()
		{

		}

		

		public void PutIn(InteractableSO interactable)
		{
			m_inventory.Add(interactable);
		}

		private List<STItemInfo> stItemList;
		public List<STItemInfo> Get_ItemList() { return stItemList; }
		public STItemInfo Get_ItemInfo(int iIndex) { return stItemList[iIndex]; }
		public int Get_ItemCount() { return stItemList.Count; }

		//private InputReader.EActionMap m_ActionMap = InputReader.EActionMap.Gameplay;


		//공용으로 다룰수 있는 구조체들을 가지고 있는 스크립트를 짜주는게 좋지 않을까?
		//C++이랑 너무 많이 다르다...
		public struct STItemInfo
		{
			public string sItemName;// None
			public int iItemID;//0
			public string sItemDescription;//None
			public bool bCheckOnHand;//false
		}
	}
}
