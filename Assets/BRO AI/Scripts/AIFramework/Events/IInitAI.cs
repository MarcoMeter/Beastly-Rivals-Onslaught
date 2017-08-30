namespace BRO.AI.Framework
{
    /// <summary>
    /// This interface has to be implemented by every AI component. Using the Init() function, the AIController will be able to trigger any initialization on the AI components.
    /// </summary>
    public interface IInitAI
    {
        void Init();
        void MyPlayerUpdate();
    }
}