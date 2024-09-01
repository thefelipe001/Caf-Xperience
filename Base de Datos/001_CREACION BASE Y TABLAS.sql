create database CafeXperienceDB

GO
USE CafeXperienceDB
GO


create table Cafeterias(
  Id int identity(1,1) NOT NULL primary key,
  Descripcion varchar(50)NOT NULL,
  Estado nchar(1)NOT NULL,
  Campus_Id int references Campus(Id)NOT NULL,
  Encargado_Id int references Usuarios(Id)NOT NULL,
)

GO

create table Campus(
  Id int identity(1,1) NOT NULL primary key,
  Descripcion varchar(50)NOT NULL,
  Estado nchar(1)NOT NULL,
)

GO

create table Usuarios(
  Id int identity(1,1) NOT NULL primary key,
  Nombre varchar(50)NOT NULL,
  Cedula varchar(50)NOT NULL,
  TipoUsuarioId int references TipoUsuarios(Id)NOT NULL,
  Limite_de_Credito int NOT NULL,
  Fecha_registro datetime NOT NULL,
  Estado nchar(1)NOT NULL,
  Correo varchar(50)NOT NULL,
  Contraseña varchar(20)NOT NULL,
)

GO

create table TipoUsuarios(
  Id int identity(1,1) NOT NULL primary key,
  Descripcion varchar(50)NOT NULL,
  Estado nchar(1)NOT NULL,
)



