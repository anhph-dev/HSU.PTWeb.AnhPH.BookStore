using HSU.PTWeb.AnhPH.BookStore.Services;

// Generate BCrypt hashes for seed data
var hasher = new PasswordHasher();

var adminHash = hasher.HashPassword("Admin@123");
var customerHash = hasher.HashPassword("Customer@123");

Console.WriteLine("-- Admin@123 hash:");
Console.WriteLine(adminHash);
Console.WriteLine();
Console.WriteLine("-- Customer@123 hash:");
Console.WriteLine(customerHash);
Console.WriteLine();
Console.WriteLine("-- SQL to reset admin password:");
Console.WriteLine($"UPDATE Users SET PasswordHash = '{adminHash}' WHERE Email = 'admin@bookstore.com';");
Console.WriteLine();
Console.WriteLine("-- SQL to reset all customer passwords:");
Console.WriteLine($"UPDATE Users SET PasswordHash = '{customerHash}' WHERE Role = 'Customer';");
