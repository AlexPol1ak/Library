# 📚 “Library” desktop application for Windows

**"Library"** is an application for efficient management of book collections and book requests. It supports both local and remote database connectivity (using MySQL as the database), making it a flexible solution for a variety of library needs.

## 📝 Project Description
This application allows you to register and process book requests, keep records of books issued, and organize a queue for popular titles when all copies are checked out. The program also provides functions for managing readers and book stock, along with building analytical charts for deeper insights.

## 🚀 Main Features
- **Reader Management**: register new readers, delete users.
- **Book Collection Management**: add and remove books from the library.
- **Request Management**: create requests, fulfill or queue requests, delete requests.
- **Analytics**: create charts to analyze book collection and user activity.

## 📂 Project Structure
The project is organized into several key packages to separate concerns and streamline functionality:
- [**Library.Domain**](https://github.com/AlexPol1ak/Library.Domain) - Contains domain entities and interfaces related to the library's core functions.
- [**Library.DAL**](https://github.com/AlexPol1ak/Library.DAL) - Data Access Layer (DAL) responsible for managing database interactions.
- [**Library.Business**](https://github.com/AlexPol1ak/Library.Business) - Business logic layer implementing the application's algorithms and core processes.

## ⚙️ Technologies and Libraries Used
- **Language and Framework**: C#, WPF
- **Database**: MySQL (supports local and remote database connections)
- **Libraries**:
  - [LiveCharts.Wpf](https://lvcharts.net/App/examples/v1/wpf/Basic%20Line%20Chart) - for analytical charting.
  - [MaterialDesign](http://materialdesigninxaml.net/) - for Material Design-inspired interface styling.
  - [Microsoft.Xaml.Behaviors.Wpf](https://github.com/microsoft/XamlBehaviorsWpf) - to add behavior for WPF components.

## 🧩 Design Patterns Used
- **MVVM** (Model-View-ViewModel) - for separating logic and interface.
- **Repository** - to control access to data.
- **Factory** - for creating object instances.
- **Unit of Work** - to manage transactions and data consistency.

---
<img src="https://github.com/user-attachments/assets/d59a9bb8-5382-4009-9c09-6d46d9a5ecf5" width="600" alt="App Animation">

> Note: The animation above showcases the key features of the application in action.
