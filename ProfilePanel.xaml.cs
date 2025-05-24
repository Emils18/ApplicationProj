using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Data;

namespace BookingSystem
{
    public partial class ProfilePanel : UserControl
    {
        public ProfilePanel()
        {
            InitializeComponent();
            Loaded += ProfilePanel_Loaded;
        }

        private void ProfilePanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProfileData();
        }

        public void LoadProfileData()
        {
            if (UserSession.UserId == 0 || string.IsNullOrEmpty(UserSession.Email))
            {
                if (DisplayNameTextBlock != null) DisplayNameTextBlock.Text = "No user logged in";
                if (FullNameTextBox != null) FullNameTextBox.Text = string.Empty;
                if (EmailTextBox != null) EmailTextBox.Text = string.Empty;
                if (NewPasswordTextBox != null) NewPasswordTextBox.Text = string.Empty;
                if (ConfirmPasswordTextBox != null) ConfirmPasswordTextBox.Text = string.Empty;
                MessageBox.Show("No active user session found. Please log in first.", "Session Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DisplayNameTextBlock != null)
                DisplayNameTextBlock.Text = UserSession.FullName;

            if (FullNameTextBox != null)
                FullNameTextBox.Text = UserSession.FullName;

            if (EmailTextBox != null)
                EmailTextBox.Text = UserSession.Email;

            if (NewPasswordTextBox != null) NewPasswordTextBox.Text = string.Empty;
            if (ConfirmPasswordTextBox != null) ConfirmPasswordTextBox.Text = string.Empty;
        }

        private void UpdateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = NewPasswordTextBox.Text.Trim();
            string confirmPassword = ConfirmPasswordTextBox.Text.Trim();

            string newEmail = EmailTextBox.Text.Trim();
            string newFullName = FullNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newFullName))
            {
                MessageBox.Show("Full Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(newEmail))
            {
                MessageBox.Show("Email cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("New password and confirm password do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                transaction = conn.BeginTransaction();

                string userLoginUpdateQuery = "UPDATE userlogin SET fullname = @newFullName, email = @newEmail";
                if (!string.IsNullOrEmpty(newPassword))
                {
                    userLoginUpdateQuery += ", password = @newPassword";
                }
                userLoginUpdateQuery += " WHERE id = @userId";

                string usersUpdateQuery = "UPDATE users SET fullname = @newFullName, email = @newEmail";
                if (!string.IsNullOrEmpty(newPassword))
                {
                    usersUpdateQuery += ", password = @newPassword";
                }
                usersUpdateQuery += " WHERE id = @userId";

                int rowsAffectedUserLogin = 0;
                int rowsAffectedUsers = 0;

                using (MySqlCommand cmd = new MySqlCommand(userLoginUpdateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@newFullName", newFullName);
                    cmd.Parameters.AddWithValue("@newEmail", newEmail);
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    }
                    cmd.Parameters.AddWithValue("@userId", UserSession.UserId);
                    rowsAffectedUserLogin = cmd.ExecuteNonQuery();
                }

                using (MySqlCommand cmd = new MySqlCommand(usersUpdateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@newFullName", newFullName);
                    cmd.Parameters.AddWithValue("@newEmail", newEmail);
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    }
                    cmd.Parameters.AddWithValue("@userId", UserSession.UserId);
                    rowsAffectedUsers = cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                if (rowsAffectedUserLogin > 0 || rowsAffectedUsers > 0)
                {
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    UserSession.FullName = newFullName;
                    UserSession.Email = newEmail;
                    LoadProfileData();
                }
                else
                {
                    MessageBox.Show("Profile update failed. User not found or no changes were made.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show($"Database error during profile update: {ex.Message}\nError Code: {ex.ErrorCode}\nPlease check your database connection and table schema.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
