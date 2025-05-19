using System;
using System.Data;
using Avalonia.Controls;
using Avalonia.Media;
using MySql.Data.MySqlClient;

namespace DeliveryApp
{
    public partial class AdministratorMainWindow : Window
    {
        private const string ConnectionString = "Server=localhost;Database=DeliveryService;User ID=root;Password=;";

        public AdministratorMainWindow()
        {
            InitializeComponent();
            CheckActiveShift();
        }

        private void CheckActiveShift()
        {
            int userId = LocalStorage.USERID;
            var currentTime = DateTime.Now;
            
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Получаем текущую активную смену
                    string shiftQuery = $@"
                        SELECT shift_id, shift_name 
                        FROM Shifts 
                        WHERE start_time <= '{currentTime:yyyy-MM-dd HH:mm:ss}' 
                          AND end_time >= '{currentTime:yyyy-MM-dd HH:mm:ss}'";

                    using (var shiftCommand = new MySqlCommand(shiftQuery, connection))
                    using (var reader = shiftCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int shiftId = reader.GetInt32("shift_id");
                            string shiftName = reader.GetString("shift_name");
                            reader.Close();

                            // Проверяем, назначен ли пользователь на текущую смену
                            string userCheckQuery = $@"
                                SELECT * FROM ShiftAssignments 
                                WHERE user_id = {userId} AND shift_id = {shiftId}";

                            using (var userCheckCommand = new MySqlCommand(userCheckQuery, connection))
                            using (var userReader = userCheckCommand.ExecuteReader())
                            {
                                if (userReader.Read())
                                {
                                    // Пользователь назначен на смену
                                    DisplayShiftInfo(shiftName, "Вы в активной смене", Colors.Green, true);
                                }
                                else
                                {
                                    // Пользователь не назначен на смену
                                    DisplayShiftInfo(shiftName, "Вы не в активной смене", Colors.Red, false);
                                }
                            }
                        }
                        else
                        {
                            // Нет активной смены на текущее время
                            DisplayShiftInfo("Нет активной смены", "Смены не найдены", Colors.Red, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayShiftInfo("Ошибка", $"Ошибка проверки смены: {ex.Message}", Colors.Red, false);
            }
        }

        private void DisplayShiftInfo(string shiftName, string statusMessage, Color color, bool isActive)
        {
            var shiftInfo = this.FindControl<TextBlock>("ShiftInfo");
            var statusText = this.FindControl<TextBlock>("StatusMessage");
            var actionButton = this.FindControl<Button>("ActionButton");

            shiftInfo.Text = $"Смена: {shiftName}";
            statusText.Text = statusMessage;
            statusText.Foreground = new SolidColorBrush(color);

            // Активируем или деактивируем кнопки в зависимости от статуса
            actionButton.IsEnabled = isActive;
        }
    }
}