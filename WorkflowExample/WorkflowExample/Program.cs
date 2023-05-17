using System;
using System.Collections.Generic;

namespace WorkflowExample
{
    public class WorkflowManager
    {
        public event EventHandler<ActionEventArgs> ActionAdded;
        public event EventHandler<ActionEventArgs> ActionCompleted;
        public event EventHandler WorkflowCompleted;

        private List<WorkflowAction> actions;
        private int currentActionIndex;

        public WorkflowManager()
        {
            actions = new List<WorkflowAction>();
            currentActionIndex = 0;
        }

        public void AddAction(WorkflowAction action)
        {
            actions.Add(action);
            OnActionAdded(action);
        }

        public void Start()
        {
            OnWorkflowStarted();

            while (currentActionIndex < actions.Count)
            {
                var currentAction = actions[currentActionIndex];
                currentAction.Execute();
                currentActionIndex++;

                OnActionCompleted(currentAction);

                if (currentActionIndex == actions.Count)
                {
                    OnWorkflowCompleted();
                }
            }
        }

        protected virtual void OnActionAdded(WorkflowAction action)
        {
            ActionAdded?.Invoke(this, new ActionEventArgs(action));
        }

        protected virtual void OnActionCompleted(WorkflowAction action)
        {
            ActionCompleted?.Invoke(this, new ActionEventArgs(action));
        }

        protected virtual void OnWorkflowStarted()
        {
            Console.WriteLine("Розпочато виконання робочого процесу");
            Console.WriteLine();
        }

        protected virtual void OnWorkflowCompleted()
        {
            WorkflowCompleted?.Invoke(this, EventArgs.Empty);
            Console.WriteLine();
            Console.WriteLine("Робочий процес завершено!");
        }
    }

    public class WorkflowAction
    {
        public string Name { get; set; }

        public WorkflowAction(string name)
        {
            Name = name;
        }

        public void Execute()
        {
            Console.WriteLine($"Виконання дiї '{Name}'...");
        }
    }

    public class ActionEventArgs : EventArgs
    {
        public WorkflowAction Action { get; set; }

        public ActionEventArgs(WorkflowAction action)
        {
            Action = action;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var workflowManager = new WorkflowManager();

            var action1 = new WorkflowAction("Дiя 1");
            var action2 = new WorkflowAction("Дiя 2");            

            workflowManager.ActionAdded += (sender, e) =>
            {
                Console.WriteLine($"Додано дiю '{e.Action.Name}' до робочого процесу");
            };

            workflowManager.ActionCompleted += (sender, e) =>
            {
                Console.WriteLine($"Завершено дiю '{e.Action.Name}'");
            };

            workflowManager.WorkflowCompleted += (sender, e) =>
            {
                Console.WriteLine("Робочий процес виконано");
            };

            workflowManager.AddAction(action1);
            workflowManager.AddAction(action2);
           
            workflowManager.Start();

            Console.ReadKey();
        }
    }
}
