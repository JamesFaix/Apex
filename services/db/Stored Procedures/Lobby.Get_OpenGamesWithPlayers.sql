SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Get_OpenGamesWithPlayers] 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT g.GameId, 
		g.GameStatusId,
		g.Description AS GameDescription, 
		g.BoardRegionCount,
		p.PlayerId,
		p.UserId,
		p.Name as PlayerName
    FROM Games g
		LEFT OUTER JOIN Players p ON g.GameId = p.GameId
    WHERE g.GameStatusId = 1 --Open
					
END
GO
