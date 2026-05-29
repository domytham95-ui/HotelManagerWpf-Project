HotelManagerWpf - C# WPF Hotel Management

Requirements:
- Windows
- .NET 8 SDK

How to run:
1. Clone this repository
2. Open terminal in the project folder
3. Run: dotnet restore
4. Run: dotnet run

Login default:
Username: admin
Password: 123456

Database:
- SQLite database is created automatically at runtime.
- Default sample rooms are seeded automatically.

Features:
- Login/Register
- Dashboard
- Room CRUD
- Customer Registration / Allot Room
- Customer Details filter: All, Current, Checked Out
- Check Out
- Employee CRUD
- SQLite database auto-created: hotel_manager_wpf.db

Rubric coverage:
- C# WPF, not WinForms
- Classes, methods, fields, properties
- Inheritance: entities inherit BaseEntity
- Polymorphism: virtual/override GetDisplayInfo, virtual CRUD service methods
- Interface: ICrudService<T>
- Abstract class: BaseEntity
- Exception handling: try-catch in service and UI logic
