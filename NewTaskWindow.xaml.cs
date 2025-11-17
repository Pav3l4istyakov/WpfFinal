using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfFinal
{
    /// <summary>
    /// Логика взаимодействия для NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        Random random = new Random();

        public NewTaskWindow()
        {
            InitializeComponent();
            TimeSpan span = new TimeSpan(random.Next(-365 * 5, 365 * 5), 0, 0, 0); 
            DateTime today = DateTime.Now;
            DateTime randomDate = today.Add(span);

            DueDatePicker.SelectedDate = randomDate; 
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            DateTime? dueDate = DueDatePicker.SelectedDate;
            string description = DescriptionTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(title))
            {
                ToDo newItem = new ToDo(title, dueDate.GetValueOrDefault(), false, description);
                MainWindow.TodoList.Add(newItem);
                Close();
            }
            else
            {
                MessageBox.Show("Нужно ввести название задачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
