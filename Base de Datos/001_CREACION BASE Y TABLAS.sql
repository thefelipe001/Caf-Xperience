USE [CafeXperienceDB]
GO

/****** Object:  Table [dbo].[Usuarios]    Script Date: 9/11/2024 10:50:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Cedula] [varchar](50) NOT NULL,
	[TipoUsuarioId] [int] NOT NULL,
	[Limite_de_Credito] [int] NOT NULL,
	[Fecha_registro] [datetime] NOT NULL,
	[Estado] [nchar](1) NOT NULL,
	[Correo] [varchar](50) NOT NULL,
	[Contraseña] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD FOREIGN KEY([TipoUsuarioId])
REFERENCES [dbo].[TipoUsuarios] ([idTipoUsuarios])
GO

-- Crear tabla TipoUsuarios
CREATE TABLE TipoUsuarios (
  idTipoUsuarios INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Descripcion VARCHAR(50) NOT NULL,
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I')) -- A = Activo, I = Inactivo
);

GO

-- Crear tabla Usuarios
CREATE TABLE Usuarios (
  idUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Cedula VARCHAR(11) NOT NULL UNIQUE, -- Cedula como campo único
  TipoUsuarioId INT NOT NULL FOREIGN KEY REFERENCES TipoUsuarios(idTipoUsuarios),
  LimiteCredito DECIMAL(18, 2) NOT NULL CHECK (LimiteCredito >= 0), -- Limite de crédito no negativo
  FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de registro por defecto a la fecha actual
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I')),
  Correo VARCHAR(50) NOT NULL UNIQUE, -- Correo como campo único
  Contraseña VARCHAR(255) NOT NULL -- Contraseña se recomienda longitud mayor para futuros hash
);

GO

-- Crear tabla Empleados
CREATE TABLE Empleados (
  idEmpleado INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Nombre VARCHAR(50) NOT NULL,
  Cedula VARCHAR(11) NOT NULL UNIQUE, -- Cedula como campo único
  TandaLabor VARCHAR(50) NOT NULL,
  PorcientoComision DECIMAL(5, 2) NOT NULL CHECK (PorcientoComision BETWEEN 0 AND 100), -- Porcentaje entre 0 y 100
  FechaIngreso DATE NOT NULL,
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I')),
  idUsuario INT FOREIGN KEY REFERENCES Usuarios(idUsuario) -- Relación con Usuarios
);

GO

-- Crear tabla Campus
CREATE TABLE Campus (
  idCampus INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Descripcion VARCHAR(50) NOT NULL,
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I'))
);

GO

-- Crear tabla Cafeterias
CREATE TABLE Cafeterias (
  idCafeteria INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Descripcion VARCHAR(50) NOT NULL,
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I')),
  idCampus INT NOT NULL FOREIGN KEY REFERENCES Campus(idCampus),
  idEncargado INT NOT NULL FOREIGN KEY REFERENCES Usuarios(idUsuario)
);

GO

-- Crear tabla Marcas
CREATE TABLE Marcas (
  idMarca INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Descripcion VARCHAR(50) NOT NULL,
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I'))
);

GO

-- Crear tabla Proveedores
CREATE TABLE Proveedores (
  idProveedor INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  NombreComercial VARCHAR(50) NOT NULL,
  RNC VARCHAR(13) NOT NULL UNIQUE, -- RNC como campo único
  FechaRegistro DATE NOT NULL DEFAULT GETDATE(), -- Fecha de registro por defecto
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I'))
);

GO

-- Crear tabla Articulos
CREATE TABLE Articulos (
  idArticulo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Descripcion VARCHAR(50) NOT NULL,
  idMarca INT NOT NULL FOREIGN KEY REFERENCES Marcas(idMarca),
  Costo DECIMAL(18, 2) NOT NULL CHECK (Costo >= 0), -- Costo no negativo
  idProveedor INT NOT NULL FOREIGN KEY REFERENCES Proveedores(idProveedor),
  Existencia INT NOT NULL CHECK (Existencia >= 0), -- Existencia no negativa
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I'))
);

GO

-- Crear tabla FacturacionArticulos
CREATE TABLE FacturacionArticulos (
  NoFactura INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  idEmpleado INT NOT NULL FOREIGN KEY REFERENCES Empleados(idEmpleado),
  idArticulo INT NOT NULL FOREIGN KEY REFERENCES Articulos(idArticulo),
  idUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuarios(idUsuario),
  FechaVenta DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de venta por defecto a la fecha actual
  MontoArticulo DECIMAL(18, 2) NOT NULL CHECK (MontoArticulo >= 0), -- Monto del artículo no negativo
  UnidadesVendidas INT NOT NULL CHECK (UnidadesVendidas > 0), -- Unidades vendidas mayor que 0
  Comentario VARCHAR(255),
  Estado NCHAR(1) NOT NULL CHECK (Estado IN ('A', 'I'))
);

GO
