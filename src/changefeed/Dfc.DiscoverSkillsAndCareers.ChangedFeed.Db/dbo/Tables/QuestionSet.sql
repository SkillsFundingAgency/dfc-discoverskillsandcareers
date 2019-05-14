CREATE TABLE [dbo].[QuestionSet] (
    [Id]             VARCHAR (500) NOT NULL,
    [LastUpdatedDt]  DATETIME      NOT NULL,
    [AssessmentType] VARCHAR (100) NULL,
    [MaxQuestions]   INT           NULL,
    [Version]        INT           NULL,
    [Title]          VARCHAR (500) NULL
);

