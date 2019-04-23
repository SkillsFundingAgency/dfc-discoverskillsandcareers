CREATE TABLE [dbo].[QuestionSet]
(
	Id varchar(500) NOT NULL,
	LastUpdatedDt datetime NOT NULL,
	AssessmentType varchar(100),
	MaxQuestions int,
	[Version] int,
	Title varchar(500)
);
