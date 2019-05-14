CREATE TABLE [dbo].[Question] (
    [Id]                        VARCHAR (150) NOT NULL,
    [LastUpdatedDt]             DATETIME      NOT NULL,
    [Text]                      VARCHAR (500) NULL,
    [TraitCode]                 VARCHAR (50)  NULL,
    [Order]                     INT           NULL,
    [IsNegative]                BIT           NULL,
    [FilterTrigger]             VARCHAR (50)  NULL,
    [SfId]                      VARCHAR (50)  NULL,
    [PositiveResultDisplayText] VARCHAR (250) NULL,
    [NegativeResultDisplayText] VARCHAR (250) NULL
);

