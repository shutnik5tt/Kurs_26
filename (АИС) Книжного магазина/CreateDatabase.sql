USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BookshopDB')
BEGIN
    CREATE DATABASE BookshopDB;
END
GO

USE BookshopDB;
GO

-- Таблица авторов
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Authors' AND xtype='U')
CREATE TABLE Authors (
    ID_author   INT IDENTITY(1,1) PRIMARY KEY,
    last_name   NVARCHAR(100) NOT NULL,
    first_name  NVARCHAR(100) NOT NULL,
    middle_name NVARCHAR(100) NULL,
    birth_date  DATETIME2    NOT NULL
);
GO

-- Таблица книг
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
CREATE TABLE Books (
    ID_book        INT IDENTITY(1,1) PRIMARY KEY,
    title          NVARCHAR(300)  NOT NULL,
    description    NVARCHAR(2000) NULL,
    release_date   DATETIME2      NOT NULL,
    unit_price     DECIMAL(18,2)  NOT NULL,
    stock_quantity INT            NOT NULL DEFAULT 0
);
GO

-- Таблица связи Автор-Книга
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuthorBooks' AND xtype='U')
CREATE TABLE AuthorBooks (
    ID_author_book INT IDENTITY(1,1) PRIMARY KEY,
    ID_author      INT NOT NULL REFERENCES Authors(ID_author),
    ID_book        INT NOT NULL REFERENCES Books(ID_book)
);
GO

-- Таблица пользователей
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    ID_user  INT IDENTITY(1,1) PRIMARY KEY,
    login    NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(256) NOT NULL,
    role     NVARCHAR(50)  NOT NULL DEFAULT 'cashier'
);
GO

-- Таблица транзакций (продаж)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
CREATE TABLE Transactions (
    ID_transaction INT IDENTITY(1,1) PRIMARY KEY,
    ID_user        INT          NOT NULL REFERENCES Users(ID_user),
    sale_date      DATETIME2    NOT NULL DEFAULT GETDATE(),
    payment_type   NVARCHAR(50) NOT NULL
);
GO

-- Таблица книг в транзакции
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BookTransactions' AND xtype='U')
CREATE TABLE BookTransactions (
    ID_book_transaction INT IDENTITY(1,1) PRIMARY KEY,
    ID_book             INT           NOT NULL REFERENCES Books(ID_book),
    ID_transaction      INT           NOT NULL REFERENCES Transactions(ID_transaction),
    quantity            INT           NOT NULL,
    total_price         DECIMAL(18,2) NOT NULL
);
GO

-- Начальный администратор (логин: admin, пароль: admin123)
IF NOT EXISTS (SELECT 1 FROM Users WHERE login = 'admin')
    INSERT INTO Users (login, password, role) VALUES ('admin', 'admin123', 'admin');
GO

PRINT 'База данных BookshopDB успешно создана.';
GO
