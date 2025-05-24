using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace BookingSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = UserName.Text.Trim();
            string password = Password.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";

            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT id, fullname, email, role FROM userlogin WHERE email = @Email AND password = @Password";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserSession.UserId = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id");
                            UserSession.FullName = reader.IsDBNull(reader.GetOrdinal("fullname")) ? string.Empty : reader.GetString("fullname");
                            UserSession.Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email");
                            UserSession.Role = reader.IsDBNull(reader.GetOrdinal("role")) ? string.Empty : reader.GetString("role");

                            MessageBox.Show($"Welcome, {UserSession.FullName}!", "Login Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            MainPanelBook mainDashboard = new MainPanelBook();
                            mainDashboard.Show();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error during login: {ex.Message}\nError Code: {ex.ErrorCode}\nPlease check your database connection string and 'userlogin' table.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
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

        private void Register_Button(object sender, RoutedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Close();
        }
    }
}
