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
    public partial class Detail : Window
    {
        public Detail()
        {
            InitializeComponent();
            ReminderTimeComboBox.SelectedIndex = 0; // Set default selected time
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            DateTime? selectedDate = ReminderDatePicker.SelectedDate;
            string selectedTime = (ReminderTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedDate.HasValue && !string.IsNullOrEmpty(selectedTime))
            {
                DateTime reminderDateTime = selectedDate.Value.Date.Add(TimeSpan.Parse(selectedTime));
                // Now you have the full reminder datetime in reminderDateTime
                MessageBox.Show("Note saved successfully with reminder at " + reminderDateTime);
            }
            else
            {
                MessageBox.Show("Please select a valid date and time.");
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel logic here
            this.Close();
        }
    }
}
