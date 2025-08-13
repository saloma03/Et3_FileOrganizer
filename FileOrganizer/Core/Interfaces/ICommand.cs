namespace FileOrganizer.Core.Interfaces
{
    public interface ICommand
    {
        void Execute();
        void Undo();

    }
}
