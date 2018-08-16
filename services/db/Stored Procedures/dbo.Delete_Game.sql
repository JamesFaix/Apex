CREATE PROCEDURE [dbo].[Delete_Game]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50000, 'Game not found', 1

	DELETE FROM [Messages] WHERE GameId = @GameId
	DELETE FROM Turns WHERE GameId = @GameId
	DELETE FROM Players WHERE GameId = @GameId
    DELETE FROM Games WHERE GameId = @GameId
END
GO
