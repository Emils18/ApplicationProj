using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Text.RegularExpressions;

namespace BookingSystem
{
    public partial class RegisterForm : Window
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string fullName = UserName.Text.Trim();
            string email = Email.Text.Trim();
            string password = Password.Password;
            string newPasswordConfirm = NewPassword.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(newPasswordConfirm))
            {
                MessageBox.Show("Please fill in all required fields.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != newPasswordConfirm)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";

            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using (MySqlCommand checkCmd = new MySqlCommand(checkEmailQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    long existingUsersCount = (long)checkCmd.ExecuteScalar();
                    if (existingUsersCount > 0)
                    {
                        MessageBox.Show("An account with this email already exists. Please use a different email or log in.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                transaction = conn.BeginTransaction();

                string insertUsersQuery = "INSERT INTO users (fullname, email, password) VALUES (@FullName, @Email, @Password)";
                using (MySqlCommand usersCmd = new MySqlCommand(insertUsersQuery, conn, transaction))
                {
                    usersCmd.Parameters.AddWithValue("@FullName", fullName);
                    usersCmd.Parameters.AddWithValue("@Email", email);
                    usersCmd.Parameters.AddWithValue("@Password", password);
                    usersCmd.ExecuteNonQuery();
                }

                int newUserId = Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID()", conn, transaction).ExecuteScalar());

                string insertUserLoginQuery = "INSERT INTO userlogin (id, email, fullname, password, role) VALUES (@Id, @Email, @FullName, @Password, @Role)";
                using (MySqlCommand userLoginCmd = new MySqlCommand(insertUserLoginQuery, conn, transaction))
                {
                    userLoginCmd.Parameters.AddWithValue("@Id", newUserId);
                    userLoginCmd.Parameters.AddWithValue("@Email", email);
                    userLoginCmd.Parameters.AddWithValue("@FullName", fullName);
                    userLoginCmd.Parameters.AddWithValue("@Password", password);
                    userLoginCmd.Parameters.AddWithValue("@Role", "user");
                    userLoginCmd.ExecuteNonQuery();
                }

                transaction.Commit();

                MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (MySqlException ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show($"Database error during registration: {ex.Message}\nError Code: {ex.ErrorCode}\nPlease check your database schema (especially 'users' and 'userlogin' tables, and 'bookings' foreign key configuration).", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show($"An unexpected error occurred during registration: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
