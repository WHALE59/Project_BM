namespace BM
{
	/// <summary>
	/// 상호작용 오브젝트들은 아래 내용을 반드시 구현해야 하고 상호작용 주체들은 이 인터페이스로 객체들과 통신.
	/// </summary>
	interface IInteractableObject
	{
		void Startnteract();
		void FinishInteract();
		void StartHover();
		void FinishHovering();

		InteractableObjectData Data { get; }
	}
}
