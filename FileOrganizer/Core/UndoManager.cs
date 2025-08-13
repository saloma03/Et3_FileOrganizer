using System.Diagnostics;
using System.Windows;
using FileOrganizer.Core.Interfaces;

namespace FileOrganizer.Core
{
    public class UndoManager
    {
        #region Fields
        private Stack<ICommand> history = new Stack<ICommand>();
        #endregion

        #region Getters
        public int getHistoryCounts()
        {
            return history.Count;
        }

        #endregion

        #region Execute
        public void Execute(ICommand command)
        {
            command.Execute();
            history.Push(command);
            //MessageBox.Show($"[UndoManager] Command added. Stack size: {history.Count}");
            Trace.WriteLine($"[UndoManager] Command added. Stack size: {history.Count}");
        }

        #endregion

        #region Undo

        public void Undo()
        {
            //MessageBox.Show($"[UndoManager] Undo called. Stack size: {history.Count}");

            if (history.Count > 0)
            {
                ICommand command = history.Pop();
                //MessageBox.Show($"[UndoManager] Executing undo for {command.GetType().Name}");
                command.Undo();
                if (history.Count> 0)
                {
                    MessageBox.Show($"[UndoManager] Undo completed. press undo again to undo the full operation");

                }
            }
            else
            {
                //MessageBox.Show("[UndoManager] No actions to undo - stack is empty");
                MessageBox.Show("No actions to undo.");
            }
        }

        #endregion
    }
}
