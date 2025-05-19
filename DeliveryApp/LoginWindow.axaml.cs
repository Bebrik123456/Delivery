using Avalonia.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SkiaSharp;

namespace DeliveryApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            var connString = "Server=localhost;Database=DeliveryService;User Id=root;Password=;";
            using (var conn = new MySqlConnection(connString))
            {
                try
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("SELECT * FROM Users WHERE username=@username AND password=@password", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        // Успешный вход
                        string role = reader.GetString("role");
                        LocalStorage.USERID = reader.GetInt32("user_Id");
                        await ShowMessage(role);
                        switch (role)
                        {
                            case "Администратор":
                                var admwin = new AdministratorMainWindow();
                                admwin.Show();
                                this.Close();
                                break;
                            case "Менеджер":
                                
                                break;
                            case "Курьер":
                                
                                break;
                            default:
                            
                                break;
                        }
                        
                        
                    }
                    else
                    {
                        // Неверный логин или пароль
                        await ShowMessage("Неверный логин или пароль.");
                    }
                }
                catch (Exception ex)
                {
                    await ShowMessage($"Ошибка: {ex.Message}");
                }
            }
        }

        private async Task ShowMessage(string message)
        {
            Console.WriteLine($"{message}");
            
        }
    }
}