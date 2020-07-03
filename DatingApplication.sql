IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DatingApplication')
	BEGIN
		CREATE DATABASE DatingApplication
	END
GO

USE DatingApplication
GO

--ALTER TABLE [Security] DROP CONSTRAINT FK_pro_username

DROP TABLE Preferences
DROP TABLE [Profile]
DROP TABLE [Security]
GO

CREATE TABLE [Security](
	Username NVARCHAR(50),
	[Password] NVARCHAR(50),
	CONSTRAINT PK_sec_username PRIMARY KEY(Username),
	
)
INSERT INTO [Security]
	VALUES
		('Tavs', 'temp');

CREATE TABLE [Profile](
	Username NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	Age INT,
	Sex NVARCHAR(50),
	AttractedTo NVARCHAR(1000),
	Height INT,
	SkinColor NVARCHAR(50),
	HairColor NVARCHAR(50),
	EyeColor NVARCHAR(50),
	Interest NVARCHAR(1000),
	CONSTRAINT PK_pro_username PRIMARY KEY(Username),
	
)
INSERT INTO [dbo].[Profile]
	VALUES 
		('Tavs', 'Tavs', 21, 'Male', 'Female', 190, 'Caucasian', 'Blonde', 'Grey;Blue', 'Reading;Programming;Cooking');
		

CREATE TABLE Preferences(
	Username NVARCHAR(50) NOT NULL,
	Age INT,
	Sex NVARCHAR(50),
	AttractedTo NVARCHAR(1000),
	Heigth INT,
	SkinColor NVARCHAR(50),
	HairColor NVARCHAR(50),
	EyeColor NVARCHAR(50),
	Interest NVARCHAR(1000)
	CONSTRAINT PK_pref_username PRIMARY KEY(Username)
	
)
INSERT INTO Preferences
	VALUES
		('Tavs', 20, 'Female', 'Male', 180, 'White', 'Any', 'Any', 'Reading;Cooking;Polishing :)')

