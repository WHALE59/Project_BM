namespace BM.Interactables
{
	/// <summary>
	/// 캐릭터가 조준선에 오브젝트를 위치시켰을 때, 검출되는 기능을 구현하고 싶으면 이 인터페이스를 상속한다.
	/// </summary>
	public interface IInteractable
	{
		public void StartHovering();
		public void FinishHovering();
	}
}