CREATE TABLE [dbo].[UserSession]
(
	Id varchar(50) NOT NULL,
	LastUpdatedDt datetime NOT NULL,
	LanguageCode varchar(50),
	QuestionSetVersion varchar(50),
	MaxQuestions int,
	CurrentQuestion int,
	IsComplete bit,
	StartedDt datetime,
	CompleteDt datetime NULL,
	AssessmentType varchar(50),
	CurrentFilterAssessmentCode varchar(50)
);

CREATE TABLE [dbo].[UserSession_Answer]
(
	Id varchar(50) NOT NULL,
	UserSessionId varchar(50),
	QuestionId varchar(50),
	QuestionNumber varchar(50),
	QuestionText nvarchar(500),
	TraitCode varchar(50),
	SelectedOption varchar(50),
	AnsweredDt datetime,
	IsNegative bit,
	QuestionSetVersion varchar(50)
);

CREATE TABLE [dbo].[UserSession_ResultStatement]
(
	Id varchar(50) NOT NULL,
	UserSessionId varchar(50),
	TextDisplayed varchar(300),
	IsTrait bit DEFAULT(0),
	IsFilter bit DEFAULT(0)
);

CREATE TABLE [dbo].[UserSession_TraitScore]
(
	Id varchar(50) NOT NULL,
	UserSessionId varchar(50),
	Trait varchar(300),
	Score int
);

CREATE TABLE [dbo].[UserSession_SuggestedJobCategory]
(
	Id varchar(50) NOT NULL,
	UserSessionId varchar(50),
	JobCategoryCode varchar(50),
	JobCategory varchar(100),
	TraitTotal int,
	JobCategoryScore decimal(18,4),
	HasCompletedFilterAssessment bit DEFAULT(0),
	FilterAssessmentQuestions int
);

CREATE TABLE [dbo].[UserSession_SuggestedJobProfile]
(
	Id varchar(50) NOT NULL,
	UserSessionId varchar(50),
	JobCategoryCode varchar(50),
	SocCode varchar(50)
);
