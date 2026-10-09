-- скрипт для ручного создания базы (то же самое делает DbInitializer при запуске)

IF DB_ID(N'UsersDb') IS NULL
    CREATE DATABASE UsersDb;
GO

USE UsersDb;
GO

-- исходная таблица из презентации
IF OBJECT_ID('Users') IS NULL
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY,
        Name NVARCHAR(100),
        Email NVARCHAR(100)
    );

    INSERT INTO Users (Id, Name, Email) VALUES
        (1, N'Иван', N'ivan@mail.ru'),
        (2, N'Анна', N'anna@mail.ru'),
        (3, N'Петр', N'petr@mail.ru');
END
GO

-- задание 1: добавляем поле Age
IF COL_LENGTH('Users', 'Age') IS NULL
    ALTER TABLE Users ADD Age INT NULL;
GO

UPDATE Users SET Age = CASE Id WHEN 1 THEN 25 WHEN 2 THEN 30 WHEN 3 THEN 19 END
WHERE Age IS NULL AND Id IN (1, 2, 3);
GO

-- задание 3: история изменений
IF OBJECT_ID('UserChanges') IS NULL
    CREATE TABLE UserChanges (
        Id INT IDENTITY PRIMARY KEY,
        UserId INT NOT NULL,
        OldName NVARCHAR(100),
        OldEmail NVARCHAR(100),
        OldAge INT,
        ChangedAt DATETIME2 NOT NULL
    );
GO
