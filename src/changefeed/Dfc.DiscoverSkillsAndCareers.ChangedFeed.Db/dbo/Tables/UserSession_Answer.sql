CREATE TABLE [dbo].[UserSession_Answer] (
    [Id]                 VARCHAR (50)   NOT NULL,
    [UserSessionId]      VARCHAR (50)   NULL,
    [QuestionId]         VARCHAR (50)   NULL,
    [QuestionNumber]     INT            NULL,
    [QuestionText]       NVARCHAR (500) NULL,
    [TraitCode]          VARCHAR (50)   NULL,
    [SelectedOption]     VARCHAR (50)   NULL,
    [AnsweredDt]         DATETIME       NULL,
    [IsNegative]         BIT            NULL,
    [QuestionSetVersion] VARCHAR (50)   NULL,
    [IsFiltered]         BIT            NULL
);

