# 📚 “Library” desktop application for Windows

**"Library ”** is an application for efficient management of book collection and book requests. It supports local and remote database connectivity, making it a flexible solution for a variety of library needs.

## 📝 Project Description
This application allows you to register and process book requests, keep records of books issued, and organize a queue for popular titles when all copies are occupied. The program also provides functions of managing readers and book stock, as well as building analytical charts.

## 🚀 Main features
- **Reader management**: register new readers, delete users.
- **Book collection management**: adding and removing books from the library.
- **Request Management**: create requests, fulfill or queue requests, delete requests.
- **Analytics**: create charts to analyze book collection and user activity.

## ⚙️ Technologies and libraries used
- **Language and framework**: C#, WPF
- **Libraries**: 
  - [LiveCharts.Wpf](https://lvcharts.net/App/examples/v1/wpf/Basic%20Line%20Chart) - analytical charting.
  - [MaterialDesign](http://materialdesigninxaml.net/) - Material Design style interface styling.
  - [Microsoft.Xaml.Behaviors.Wpf](https://github.com/microsoft/XamlBehaviorsWpf) - adding behavior for WPF components.

## 🧩 Design patterns used
- **MVVM** (Model-View-View-ViewModel) - for separating logic and interface.
- **Repository** - to control access to data.
- **Factory** - for creating object instances.
- **Unit of Work** - to manage transactions and data consistency.
