# SoNetProject
The project is some outline of a social network. The application is written in C# using WPF, MVVM pattern. The Entity Framework is used to communicate with the MSSQL Server database. Sending messages is done using WCF. The Google Drive API was used to work with files.
Important points:
In the curs project, in the app.config file, you must specify the server name in the database connection string.
To send messages, you need to build the ChatHost project. Run the .exe of this project as an administrator. Now we can start two instances of curs and forward messages. Necessarily! Users must be in a collaborative chat at the same time as messages are not saved