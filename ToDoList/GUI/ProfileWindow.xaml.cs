using Repositories.Interfaces;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using System.Diagnostics;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly ProfileService _profileService;
        public ObservableCollection<Profile> Profiles { get; set; }

        public static bool isShow = false;

        public ProfileWindow()
        {
            InitializeComponent();
            IProfileRepository profileRepo = new ProfileRepository(new ToDoListDbContext());
            INoteRepository noteRepo = new NoteRepository(new ToDoListDbContext());
            _profileService = new ProfileService(profileRepo, noteRepo);
            Profiles = new ObservableCollection<Profile>();
            DataContext = this;
            LoadProfileListBox();
            Closing += (sender, e) =>
            {
                Application.Current.Shutdown();
            };

        }

        private void CreateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string profilename = ProfileNameTextBox.Text;
            
            if (!string.IsNullOrWhiteSpace(profilename) && profilename != "Enter profile name" && Profiles.ToList().FirstOrDefault(x => x.ProfileName == profilename) == null && _profileService.AddProfile(new Repositories.Entities.Profile { ProfileName = profilename }))
            {
             
                Profiles.Add(_profileService.GetProfilesByName(profilename).FirstOrDefault(x => x.ProfileName == profilename));
                ProfileNameTextBox.Clear();
                ProfileNameTextBox.Text = "Enter profile name";
                ProfileNameTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                MessageBox.Show("Profile name cannot be empty, default, or duplicate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ExitButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ProfileNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ProfileNameTextBox.Text == "Enter profile name")
            {
                ProfileNameTextBox.Text = "";
                ProfileNameTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void ProfileNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProfileNameTextBox.Text))
            {
                ProfileNameTextBox.Text = "Enter profile name";
                ProfileNameTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
        private void ProfilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadProfileListBox()
        {
            var profiles = _profileService.GetAllProfiles().ToList();
            Profiles.Clear();
            foreach (var profile in profiles)
            {
                Profiles.Add(profile);
            }
        }

        private void ProfilesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProfilesListBox.SelectedItem != null)
            {
                Profile selectedProfile = ((Profile)ProfilesListBox.SelectedItem);
                HomeWindow home = new HomeWindow();
                ProfileInfo.UserProfile = selectedProfile;
                this.Hide();
                home.Show();
                HomeWindow.isShow = true;
            }
        }

        private void ProfilesListBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ProfilesListBox.SelectedItem != null)
            {
                string selectedProfile = ((Profile)ProfilesListBox.SelectedItem).ProfileName;
                MessageBoxResult result = MessageBox.Show($"Do you want to delete {selectedProfile}?", "Profile Selected", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var profile = _profileService.GetProfilesByName(selectedProfile).FirstOrDefault(x => x.ProfileName == selectedProfile);
                    if (profile != null && _profileService.DeleteProfile(profile))
                    {
                        Profiles.Remove(profile);
                        MessageBox.Show($"{profile.ProfileName} has been deleted.", "Profile Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete {selectedProfile}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        
    }
}
