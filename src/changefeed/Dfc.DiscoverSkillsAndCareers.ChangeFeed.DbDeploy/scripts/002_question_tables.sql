CREATE TABLE [dbo].[Question]
(
	Id varchar(150) NOT NULL,
	LastUpdatedDt datetime NOT NULL,
	[Text] varchar(500),
	TraitCode varchar(50),
	[Order] int,
	IsNegative bit,
	FilterTrigger varchar(50),
	SfId varchar(50),
	PositiveResultDisplayText varchar(250),
	NegativeResultDisplayText varchar(250)
);

CREATE TABLE [dbo].[Question_ExcludeJobProfile]
(
	Id varchar(50) NOT NULL,
	QuestionId varchar(150),
	JobProfile varchar(100)
);