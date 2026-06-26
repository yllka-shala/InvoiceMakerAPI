# 🚀 InvoiceMakerAPI
A backend Web API built with .NET 9 and C# helping users to manage invoices for their business.

## ✨ Key Features
InvoiceMakerAPI is a system that allows users to:

* Register.
* Authenticate.
* Create Invoices.
* Generate Invoices.
* Includes tax, discount and subtotal logic.
* Multi-currency support. 
* Get all or individual invoices.
* Delete Invoices.
* Generate several reports (Revenue summary, Revenue by month, Invoice status breakdown, Top clients).

## 🛠 Tech Stack

* **Language:** C#
* **Framework:** .NET 9 / ASP.NET Core WEB API
* **Database:** SQL Server
* **Authentication & Security:** ASP.NET Core Identity, JWT
* **ORM:** Entity Framework Core
* **Files library:** QuestPdf for invoice design and generation.
* **API Documentation:** Swagger

## 🚀 Getting Started
To run this project locally, follow these steps:

1. **Clone the repo:**
git clone https://github.com/yllka-shala/InvoiceMakerAPI.git

2. **Open the solution:**
Open `InvoiceMakerAPI.sln` in Visual Studio.

2. **Migrate the database:**
Perform migration and create the database by using the commands
"Add-Migration Initial" and "Update-Database" in Package Manager Console.

4. **Run:**
Press `F5` to build and launch the app.
