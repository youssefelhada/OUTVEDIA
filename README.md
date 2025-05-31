# Final Project

This is a .NET Core web application that implements user authentication and authorization using ASP.NET Core Identity.

## Features

- User authentication and authorization
- Role-based access control
- Email service integration using Brevo
- SQL Server database integration

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server
- Visual Studio 2022 or Visual Studio Code

## Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json` to point to your SQL Server instance
3. Update the Brevo API key in `appsettings.json`
4. Run the following commands in the terminal:

```bash
dotnet restore
dotnet build
dotnet run
```

## Configuration

The application requires the following configuration in `appsettings.json`:

- `ConnectionStrings:DefaultConnection`: SQL Server connection string
- `BrevoSettings:Apikey`: Brevo API key for email service

## Project Structure

- `Controllers/`: Contains the application's controllers
- `Models/`: Contains the data models
- `Services/`: Contains business logic and services
- `Views/`: Contains the Razor views
- `wwwroot/`: Contains static files (CSS, JavaScript, images)

## Security

The application implements the following security measures:

- Password hashing using ASP.NET Core Identity
- HTTPS enforcement
- Secure cookie handling
- Cross-Site Request Forgery (CSRF) protection

## License

This project is licensed under the MIT License - see the LICENSE file for details. 