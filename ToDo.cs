using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinal
{
    public class ToDo
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public ToDo(string title, DateTime dueDate, bool isCompleted, string description)
        {
            Title = title;
            DueDate = dueDate;
            IsCompleted = isCompleted;
            Description = description;
        }
    }
}
