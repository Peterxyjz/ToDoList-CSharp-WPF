-- Tạo cơ sở dữ liệu
-- USE master
-- DROP DATABASE ToDoListDB
GO
CREATE DATABASE ToDoListDB;
GO
-- Sử dụng cơ sở dữ liệu vừa tạo
USE ToDoListDB;
GO
-- Tạo bảng Profile
CREATE TABLE Profile
(
  Profile_ID INT IDENTITY(1,1) PRIMARY KEY,
  Profile_Name NVARCHAR(100) NOT NULL
);
GO
-- Tạo bảng Note với cấu trúc được sửa
CREATE TABLE Note
(
  Note_ID INT IDENTITY(1,1) PRIMARY KEY,
  Profile_ID INT NOT NULL,
  Title NVARCHAR(50) NOT NULL,
  [Description] NVARCHAR(100),
  Modified_Date DATETIME NOT NULL,
  [Status] NVARCHAR(30),
  [Time] DATETIME NOT NULL, --Dùng để nhắc
  FOREIGN KEY (Profile_ID) REFERENCES Profile(Profile_ID)
);
GO
