using System;
using System.Collections.Generic;

public class TaskScheduler<TTask, TPriority>
{
    private SortedDictionary<TPriority, Queue<TTask>> taskQueue = new SortedDictionary<TPriority, Queue<TTask>>();
    private Func<TTask, TPriority> prioritySelector;

    public TaskScheduler(Func<TTask, TPriority> prioritySelector)
    {
        this.prioritySelector = prioritySelector;
    }

    public void AddTask(TTask task)
    {
        TPriority priority = prioritySelector(task);

        if (!taskQueue.ContainsKey(priority))
        {
            taskQueue[priority] = new Queue<TTask>();
        }

        taskQueue[priority].Enqueue(task);
    }

    public void ExecuteNext(TaskExecution<TTask> executionDelegate)
    {
        if (taskQueue.Count == 0)
        {
            Console.WriteLine("No tasks in the queue.");
            return;
        }

        TPriority highestPriority = taskQueue.Keys.Max();
        TTask taskToExecute = taskQueue[highestPriority].Dequeue();

        executionDelegate(taskToExecute);
    }

    public void ShowQueue()
    {
        foreach (var kvp in taskQueue)
        {
            Console.WriteLine($"Priority: {kvp.Key}");
            foreach (var task in kvp.Value)
            {
                Console.WriteLine($"Task: {task}");
            }
        }
    }
}

public delegate void TaskExecution<TTask>(TTask task);

class Program
{
    static void Main()
    {
        TaskScheduler<string, int> scheduler = new TaskScheduler<string, int>(task => int.Parse(task.Split('-')[1]));

        scheduler.AddTask("Task-3");
        scheduler.AddTask("Task-1");
        scheduler.AddTask("Task-2");
        scheduler.AddTask("Task-5");
        scheduler.AddTask("Task-4");

        Console.WriteLine("Initial Queue:");
        scheduler.ShowQueue();

        Console.WriteLine("\nExecuting next task...");
        scheduler.ExecuteNext(task => Console.WriteLine($"Executing task: {task}"));

        Console.WriteLine("\nUpdated Queue:");
        scheduler.ShowQueue();
    }
}

