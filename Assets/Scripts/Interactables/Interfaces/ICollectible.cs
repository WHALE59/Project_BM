namespace BM.InteractableObjects
{
	public interface ICollectible : IEquippable
	{
		void Enable();
		void Disable();
	}
}