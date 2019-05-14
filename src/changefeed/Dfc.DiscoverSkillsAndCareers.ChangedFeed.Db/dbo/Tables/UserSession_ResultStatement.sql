CREATE TABLE [dbo].[UserSession_ResultStatement] (
    [Id]            VARCHAR (50)  NOT NULL,
    [UserSessionId] VARCHAR (50)  NULL,
    [TextDisplayed] VARCHAR (300) NULL,
    [IsTrait]       BIT           DEFAULT ((0)) NULL,
    [IsFilter]      BIT           DEFAULT ((0)) NULL
);

