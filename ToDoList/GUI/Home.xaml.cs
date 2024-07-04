using Repositories.Entities;
using Services;
using Services.Interfaces;
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

namespace GUI
{
    public partial class Home : Window
    {
        private readonly INoteService _noteService;
        public Home(INoteService noteService)
        {
            InitializeComponent();
            _noteService = noteService;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNote();
        }

        private void NotesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
