CREATE TABLE [dbo].[UserSession] (
    [Id]                          VARCHAR (50) NOT NULL,
    [LastUpdatedDt]               DATETIME     NOT NULL,
    [LanguageCode]                VARCHAR (50) NULL,
    [QuestionSetVersion]          VARCHAR (50) NULL,
    [MaxQuestions]                INT          NULL,
    [CurrentQuestion]             INT          NULL,
    [IsComplete]                  BIT          NULL,
    [StartedDt]                   DATETIME     NULL,
    [CompleteDt]                  DATETIME     NULL,
    [AssessmentType]              VARCHAR (50) NULL,
    [CurrentFilterAssessmentCode] VARCHAR (50) NULL
);

