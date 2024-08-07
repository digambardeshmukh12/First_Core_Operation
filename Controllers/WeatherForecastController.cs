using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using Npgsql;
using System.Threading.Tasks;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Database=DD;Username=postgres;Password=123456;";

        // Model for user registration
        public class RegisterUserModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        // Model for user login
        public class LoginUserModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // Model for updating user data
        public class UpdateUserModel
        {
            public int UserId { get; set; }
            public string NewUsername { get; set; }
            public string NewPassword { get; set; }
            public string NewEmail { get; set; }
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] RegisterUserModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new NpgsqlCommand("INSERT INTO login_details (username, password, email) VALUES (@username, @password, @Email)", connection);
                    command.Parameters.AddWithValue("username", model.Username);
                    command.Parameters.AddWithValue("password", model.Password);
                    command.Parameters.AddWithValue("email", model.Email);

                    await command.ExecuteNonQueryAsync();
                }

                return Ok("User added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid login data.");
            }

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new NpgsqlCommand("SELECT COUNT(1) FROM login_details WHERE username = @username AND password = @password", connection);
                    command.Parameters.AddWithValue("username", model.Username);
                    command.Parameters.AddWithValue("password", model.Password);

                    var result = (long)await command.ExecuteScalarAsync();

                    if (result == 1)
                    {
                        return Ok("Login successful.");
                    }
                    else
                    {
                        return Unauthorized("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid update data.");
            }

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new NpgsqlCommand("UPDATE login_details SET username = @newUsername, password = @newPassword, email = @newEmail WHERE user_id = @userId", connection);
                    command.Parameters.AddWithValue("newUsername", model.NewUsername);
                    command.Parameters.AddWithValue("newPassword", model.NewPassword);
                    command.Parameters.AddWithValue("newEmail", model.NewEmail);
                    command.Parameters.AddWithValue("userId", model.UserId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Ok("User updated successfully.");
                    }
                    else
                    {
                        return NotFound("User not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new NpgsqlCommand("DELETE FROM login_details WHERE user_id = @userId", connection);
                    command.Parameters.AddWithValue("userId", userId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Ok("User deleted successfully.");
                    }
                    else
                    {
                        return NotFound("User not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
