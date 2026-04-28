using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp
{
    internal class TodoTask
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; }
        public TodoTask(string title)
        {
            Title = title;
            IsDone = false;
            CreatedAt = DateTime.Now;
        }
    }
}
