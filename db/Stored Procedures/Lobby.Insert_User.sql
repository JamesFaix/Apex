SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO



CREATE PROCEDURE [Lobby].[Insert_User]
	@Name NVARCHAR(50),
	@RoleId TINYINT,
	@Password NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Users (
		[Name], 
		CreatedOn, 
		RoleId, 
		[Password], 
		FailedLoginAttempts, 
		LastFailedLoginAttemptOn)
	VALUES (
		@Name, 
		GETUTCDATE(), 
		@RoleId, 
		@Password,
		0,
		NULL)

	SELECT SCOPE_IDENTITY()
END
GO
