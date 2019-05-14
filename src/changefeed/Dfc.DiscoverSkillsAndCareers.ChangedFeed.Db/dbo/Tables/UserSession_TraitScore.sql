CREATE TABLE [dbo].[UserSession_TraitScore] (
    [Id]            VARCHAR (50)  NOT NULL,
    [UserSessionId] VARCHAR (50)  NULL,
    [Trait]         VARCHAR (300) NULL,
    [Score]         INT           NULL
);

