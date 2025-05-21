using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia;

namespace DeliveryApp
{
    public partial class EmployeesWindow : Window
    {


        public EmployeesWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            // 1. Очищаем список
            OrdersListBox.Items.Clear();

            // 2. Получаем данные из БД
            var orders = GetOrdersFromDatabase();

            // 3. Заполняем ListBox вручную
            foreach (var order in orders)
            {
                // Формируем строку для отображения
                string a;


                string orderInfo = $"{order.UserId} | {order.Username} | {order.Role}| {order.Status} ";
                OrdersListBox.Items.Add(orderInfo);
            }
        }

        private List<Employee> GetOrdersFromDatabase()
        {
            var orders = new List<Employee>();

            var conn = new MySqlConnection("Server=localhost;Database=Provider;User Id=root;Password=;");
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Provider.Task", conn);
                var reader = cmd.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        orders.Add(new Employee()
                        {
                            UserId = reader.GetInt32("user_id "),
                            Username = reader.GetString("username "),
                            Role = reader.GetString("role"),
                            Status = reader.GetString("status")

                        });
                    }
                }
            }

            return orders;
        }


        public class Employee
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            
            public string password { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }
        }
    }
}