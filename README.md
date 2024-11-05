# 📚 “Library” desktop application for Windows

**"Library"** is an application for efficient management of book collections and book requests. It supports both local and remote database connectivity (using MySQL as the database), making it a flexible solution for a variety of library needs.

## 📝 Project Description
This application allows you to register and process book requests, keep records of books issued, and organize a queue for popular titles when all copies are checked out. The program also provides functions for managing readers and book stock, along with building analytical charts for deeper insights.

## 🚀 Main Features
- **Reader Management**: register new readers, delete users.
- **Book Collection Management**: add and remove books from the library.
- **Request Management**: create requests, fulfill or queue requests, delete requests.
- **Analytics**: create charts to analyze book collection and user activity.

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

![App Animation](https://github.com/user-attachments/assets/f773f02d-e025-49d9-8f30-43013d613d99 | width=500)

> Note: The animation above showcases the key features of the application in action.
