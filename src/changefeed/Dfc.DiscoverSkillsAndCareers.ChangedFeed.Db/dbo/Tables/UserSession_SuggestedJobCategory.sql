CREATE TABLE [dbo].[UserSession_SuggestedJobCategory] (
    [Id]                           VARCHAR (50)    NOT NULL,
    [UserSessionId]                VARCHAR (50)    NULL,
    [JobCategoryCode]              VARCHAR (50)    NULL,
    [JobCategory]                  VARCHAR (100)   NULL,
    [TraitTotal]                   INT             NULL,
    [JobCategoryScore]             DECIMAL (18, 4) NULL,
    [HasCompletedFilterAssessment] BIT             DEFAULT ((0)) NULL,
    [FilterAssessmentQuestions]    INT             NULL
);

