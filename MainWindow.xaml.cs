using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<ToDo> TodoList { get; set; } = new ObservableCollection<ToDo>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadFromJsonFile();
            TodoList.Clear();
            UpdateProgress();
            TodoList.Add(new ToDo("Приготовить покушать", new DateTime(2024, 1, 15), true, "Нет описания"));
            TodoList.Add(new ToDo("Поработать", new DateTime(2024, 1, 20), false, "Съездить на совещание в Москву"));
            TodoList.Add(new ToDo("Отдохнуть", new DateTime(2024, 2, 1), false, "Съездить в отпуск в Сочи"));
            TaskListListBox.ItemsSource = TodoList;
        }

        private void UpdateProgress()
        {
            int totalTasks = TodoList.Count;
            int completedTasks = TodoList.Count(x => x.IsCompleted);
            TaskProgressBar.Minimum = 0;
            TaskProgressBar.Maximum = totalTasks;
            TaskProgressBar.Value = completedTasks;
            ProgressText.Text = $"{completedTasks} / {totalTasks}";
        }

        private void OpenNewTaskWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new NewTaskWindow();
            window.Owner = this;
            window.ShowDialog();
            UpdateProgress();
        }

        private void DeleteSelectedExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = TaskListListBox.SelectedItem as ToDo;
            if (selectedItem != null && MessageBox.Show("Вы действительно хотите удалить данную задачу?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TodoList.Remove(selectedItem);
                UpdateProgress();
            }
        }

        private void CanDeleteSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TaskListListBox.SelectedItems.Count > 0;
        }

        private void SaveTxtFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Файлы JSON (*.json)|*.json",
                DefaultExt = ".txt",
                OverwritePrompt = true,
                CheckPathExists = true,
                CreatePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Сохранить список дел"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(saveDialog.FileName)?.TrimStart('.');

                if (extension.Equals("txt"))
                {
                    SaveAsTxt(saveDialog.FileName);
                }
                else if (extension.Equals("json"))
                {
                    SaveAsJson(saveDialog.FileName);
                }
                else
                {
                    MessageBox.Show("Неподдерживаемый формат файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveAsTxt(string filename)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var todo in TodoList)
            {
                sb.AppendLine($"{todo.Title}, {todo.DueDate.ToString("dd-MM-yyyy")}, {todo.Description}");
            }

            try
            {
                File.WriteAllText(filename, sb.ToString());
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка");
            }
        }

        private void SaveAsJson(string filename)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(sw, TodoList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в JSON: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveJsonFile();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.New.Execute(null, this);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Save.Execute(null, this);
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var taskToRemove = button?.DataContext as ToDo;

            if (taskToRemove != null)
            {
                TodoList.Remove(taskToRemove);
                UpdateProgress();
            }
        }

        private void SaveJsonFile()
        {
            try
            {
                string dirPath = Directory.GetCurrentDirectory() + @"\Files";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string filePath = dirPath + @"\todos.json";

                using (StreamWriter sw = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(sw, TodoList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в JSON: {ex.Message}");
            }
        }

        private void LoadFromJsonFile()
        {
            string filePath = Directory.GetCurrentDirectory() + @"\Files\todos.json";

            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    TodoList.Clear();

                    foreach (var item in (ObservableCollection<ToDo>)serializer.Deserialize(sr, typeof(ObservableCollection<ToDo>)))
                    {
                        TodoList.Add(item);
                    }
                }
            }
        }

        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            UpdateProgress();
        }

        private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
        {
            UpdateProgress();
        }
    }
}

